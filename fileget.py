#!/usr/bin/env python3.8
# -*- coding: utf-8 -*-
"""
Brief:   Assignment for the IPK BUT FIT course 2020/2021.
Author:  Marek "Wecros" Filip <xfilip46>
Date:    2020/03/23
Details: Network client that downloads files from a server and saves it
         in local directory. Uses NSP and FSP protocols.

Edge cases behaviour
- GET ALL: Only supports the default behaviour
    - Using "fsp://file.server/*" will download all server files."
        - The whole file structure is saved.
    - Using "fsp://file.server/acrobat/*" tries to download file name '*' from acrobat folder.
- GET
    - Only complete file paths are allowed, cannot download directories.

I have defined exit codes like this:
0 - success
1 - not found error
2 - invalid program arguments
3 - refused/lost connection error
4 - other errors
"""

import argparse
import os
import re
import sys
from pathlib import Path
from urllib.parse import urlparse

from socket_api import tcp_connection, udp_connection


def enum(*sequential, **named):
    enums = dict(zip(sequential, range(len(sequential))), **named)
    return type("Enum", (), enums)


Code = enum(
    "SUCCESS",
    "NOT_FOUND",
    "INVALID_ARG",
    "REFUSED_CONNECTION",
    "OTHER",
)
AGENT = "xfilp46"


def print_usage():
    ...


def main():
    address, protocol, server, filepath = parse_arguments()

    with udp_connection(address) as udp:
        address = get_tcp_server_address(udp, server)
    with tcp_connection(address) as tcp:
        handle_tcp_connection(tcp, server, filepath)

    sys.exit(Code.SUCCESS)


def get_tcp_server_address(udp, server):
    """Return new address got from the translation server."""
    msg = f"WHEREIS {server}"
    try:
        udp.send(msg)
        received_msg = udp.recieve()
    except ConnectionError:
        error_exit(
            Code.REFUSED_CONNECTION, f"{udp.addr_conv()} connection refused or lost."
        )
    except BlockingIOError as e:
        error_exit(
            Code.REFUSED_CONNECTION,
            str(e) + f"\nTimeout with {udp.addr_conv()} reached.",
        )
    except RuntimeError as e:
        error_exit(Code.OTHER, str(e))

    rc, msg = received_msg.split(" ", 1)
    if rc == "OK":
        host, port = msg.split(":")
        port = int(port)
        address = (host, port)
        return address
    elif rc == "ERR" and msg == "Not Found":
        error_exit(Code.NOT_FOUND, f"{server} not found!")
    error_exit(Code.OTHER, "Invalid response got from server.")


def handle_tcp_connection(tcp, server, filepath):
    if filepath == "*":
        save_every_file_from_server(tcp, server)
    else:
        save_one_file_from_server(tcp, server, filepath)


def save_every_file_from_server(tcp, server):
    header, index = get_file_from_server(tcp, server, "index")
    for file in index.decode().splitlines():
        header, content = get_file_from_server(tcp, server, file)
        create_dir_if_necessary(file)
        save_content_to_file(content, file)


def save_one_file_from_server(tcp, server, filepath):
    header, content = get_file_from_server(tcp, server, filepath)
    save_content_to_file(content, os.path.basename(filepath))


def get_file_from_server(tcp, server, filepath):
    msg = f"GET {filepath} FSP/1.0\r\nHostname: {server}\r\nAgent:{AGENT}\r\n\r\n"
    try:
        tcp.send(msg)
        header, content = tcp.receive_with_header()
    except ConnectionError as e:
        error_exit(Code.REFUSED_CONNECTION, str(e))
    except BlockingIOError as e:
        error_exit(
            Code.REFUSED_CONNECTION,
            str(e) + f"\nTimeout with {tcp.addr_conv()} reached.",
        )
    except RuntimeError as e:
        error_exit(Code.OTHER, str(e))

    validate_header(header, server, filepath)
    return header, content


def validate_header(header, server, filepath):
    if re.search(r"FSP/1\.0 Not Found\r\nLength:\s*\d+", header):
        error_exit(Code.NOT_FOUND, f"File {filepath} not found on {server}")
    if not re.search(r"FSP/1\.0 Success\r\nLength:\s*\d+", header):
        error_exit(Code.OTHER, "Invalid response got from server.")


def create_dir_if_necessary(filepath):
    dirname = os.path.dirname(filepath)
    if dirname:
        Path(dirname).mkdir(parents=True, exist_ok=True)


def save_content_to_file(content, filepath):
    with open(filepath, "wb") as file:
        file.write(content)


def parse_arguments():
    parser = argparse.ArgumentParser(
        description="Download file from server and save it in local directory."
    )
    parser.add_argument(
        "-n",
        metavar="NAMESERVER",
        type=str,
        required=True,
        help="IP address and port number of named server. IP:PORT format.",
    )
    parser.add_argument(
        "-f",
        metavar="SURL",
        type=str,
        required=True,
        help="Simplified URL of the downloaded file. URL protocol is always FSP."
        "fsp://SERVER_NAME/PATH format.",
    )
    global print_usage
    print_usage = parser.print_usage

    args = parser.parse_args()
    return get_options(args)


def get_options(args):
    try:
        host, port = args.n.split(":")
        port = int(port)
        address = host, port
    except ValueError:
        error_exit(Code.INVALID_ARG, "NAMESERVER argument in invalid format.")

    surl = urlparse(args.f)
    protocol, server, filepath = surl.scheme, surl.netloc, surl.path[1:]
    if protocol != "fsp" or not server or not filepath:
        error_exit(Code.INVALID_ARG, "SURL argument in invalid format.")

    return address, protocol, server, filepath


def error_exit(code, msg):
    global print_usage
    print_usage()
    sys.stderr.write(str(msg) + "\n")
    sys.exit(code)


if __name__ == "__main__":
    main()
