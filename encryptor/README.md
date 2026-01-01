
# AES‑CBC Encryption/Decryption Tool

## Overview

Tool to encrypt and decrypt strings or files using **AES‑CBC** with **PKCS7 padding**. The encryption key and Initialization Vector (IV) are manually entered each time. Encrypted output is returned as **Base64** text.

## Usage

Download and Execute the `tool.exe`
### Strings

1. **Run the tool** → Choose the operation:  
   - **E** for encrypting a string  
   - **D** for decrypting a string

2. **Enter the key and IV** (16 characters each).

3. **Input**:  
   - For encryption (`E`), provide the plaintext to encrypt.  
   - For decryption (`D`), provide the Base64-encoded ciphertext.

4. The **output** will be displayed in the console.

### Files

1. **Choose the operation**:  
   - **EF** for encrypting a file  
   - **DF** for decrypting a file

2. **Enter the key and IV** (16 characters each).

3. **Provide file paths**:
   - For encryption (`EF`): Select an input file and specify the output file path where the Base64-encoded text will be written.
   - For decryption (`DF`): Provide the input file containing the Base64 ciphertext and specify the output file path where the decrypted raw file will be saved.

### Notes

- **Key**: The key string is hashed using **SHA‑256** to produce a 256-bit AES key.
- **IV**: The Initialization Vector (IV) must be exactly **16 characters** long.
- **Important**: If the **key or IV** is incorrect, decryption will either fail or produce garbage output.
- **Encrypted Files**: Encrypted files are written as **Base64 text**. Decrypted files restore the original raw content.

## Example Commands

### Encrypting a string
```bash
E
Enter key: mysecretkey1234
Enter IV: myiv1234
Enter plaintext: Hello, world!
````

### Decrypting a string

```bash
D
Enter key: mysecretkey1234
Enter IV: myiv1234
Enter Base64 ciphertext: <Base64 cipher text here>
```

### Encrypting a file

```bash
EF
Enter key: mysecretkey1234
Enter IV: myiv1234
Enter input file path: input.txt
Enter output file path: encrypted_output.txt
```

### Decrypting a file

```bash
DF
Enter key: mysecretkey1234
Enter IV: myiv1234
Enter input file path: encrypted_output.txt
Enter output file path: decrypted_output.txt
```