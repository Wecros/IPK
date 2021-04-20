- Brief:   Assignment for the IPK BUT FIT course 2020/2021.
- Author:  Marek "Wecros" Filip (xfilip46)
- Date:    2020/03/23
- Details: Network client that downloads files from a server and saves it in local directory. Uses made-up NSP and FSP protocols.

### Python Version IMPORTANT
Please use `python3.8` version when testing on merlin.fit.vutbr.cz.

### Edge Cases Behaviour
- GET ALL: Only supports the default behaviour
    - Using "fsp://file.server/*" will download all server files."
        - The whole file structure is saved.
    - Using "fsp://file.server/acrobat/*" tries to download file name '*' from acrobat folder.
- GET
    - Only complete file paths are allowed, cannot download directories.

### Defined Exit Codes
- 0: success
- 1: not found error
- 2: invalid program arguments
- 3: refused/lost connection error
- 4: other errors

