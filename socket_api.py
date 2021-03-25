# -*- coding: utf-8 -*-
"""
Brief:   Assignment for the IPK BUT FIT course 2020/2021.
Author:  Marek "Wecros" Filip <xfilip46>
Date:    2020/03/23
Details: My own custom API to ease the working with python's socket API.
"""

import re
import socket
from contextlib import contextmanager


@contextmanager
def tcp_connection(address):
    sock = SocketTCP(address)
    try:
        yield sock
    finally:
        sock.close()


@contextmanager
def udp_connection(address):
    sock = SocketUDP(address)
    try:
        yield sock
    finally:
        sock.close()


class Socket:
    __slots__ = "sock"
    TIMEOUT_IN_SECONDS = 30

    def __init__(self, family, type, address):
        self.family = family
        self.type = type
        self.address = address

    def create_new_socket(self):
        try:
            self.sock = socket.socket(self.family, self.type)
            self.sock.settimeout(self.TIMEOUT_IN_SECONDS)
            self.sock.connect(self.address)
        except socket.error as e:
            raise ConnectionError(
                str(e) + f"\nConnection refused or lost with {self.addr_conv()}."
            )
        return self.sock

    def send(self, msg):
        """Send a message to the server.

        :param msg: Unencoded message.
        :type msg: str.
        :return: Number of bytes sent.
        :rtype: int.
        """
        self.sock = self.create_new_socket()
        msg = msg.encode()
        msglen = len(msg)
        totalsent = 0

        while totalsent < msglen:
            sent = self.sock.send(msg[totalsent:])
            if sent == 0:
                raise RuntimeError("Socket connection has been broken.")
            totalsent += sent
        return totalsent

    def recieve(self, bytes=4096, headers=False):
        """Receive a message from the server.

        :return: Decoded message.
        :rtype str.
        """
        content = self._get_content(bytes)
        return b"".join(content).decode()

    def receive_with_header(self, bytes=4096):
        header = self.sock.recv(bytes)
        content = self._get_content(bytes)

        header, content = header.decode(), b"".join(content).decode()
        return self._validate_header(header, content)

    def _get_content(self, bytes):
        content = []
        while chunk := self.sock.recv(bytes):
            content.append(chunk)
            if len(chunk) != bytes:
                break
        self.close()
        return content

    @staticmethod
    def _validate_header(header, content):
        """Handle case when reading of header results in reading of content."""
        header_extra = re.search(r"(Length:\d+\r\n\r\n)(.+)", header, flags=re.DOTALL)
        if header_extra:
            header = header[: header_extra.start(1)] + header_extra.group(1)
            content = header_extra.group(2) + content

        return header, content

    def addr_conv(self):
        """Convert IP, PORT tuple into readable string."""
        return f"{self.address[0]}:{self.address[1]}"

    def close(self):
        self.sock.close()


class SocketTCP(Socket):
    def __init__(self, address):
        super().__init__(socket.AF_INET, socket.SOCK_STREAM, address)


class SocketUDP(Socket):
    def __init__(self, address):
        super().__init__(socket.AF_INET, socket.SOCK_DGRAM, address)
