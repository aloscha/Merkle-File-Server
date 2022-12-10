# Merkle File Server

Proof of concept for Trustless File Server in C#.

The server expose a `hashes` HTTP endpoint to serve as something to represent this trusted source.
The server also expose a `piece` HTTP endpoint can be used to download each piece of the file, along with a *proof* for the piece which they can use to reconstruct the root hash and verify that the provided piece is correct. By doing this for all the pieces, they can reconstruct the whole file.

## Task/Implementation thoughts

* The MertkeTree was implemented from scratch, so no extra library been used. Generally I decided for that because I have controll of everything what's happening, so no extra calculations should be done
* The server was written with some level of abtraction (especially in MerkleLeaf/MerkleNode implementation)
* The entire app was splitted to smaller chunks responsible for their own task (like FileManager or TreeManager)
* Because this task is not for memory testing I decided to:
  * Use singleton class for building MerkleTrees on the beggining (when server starts)
    * For every file I build 1 MerkleTree and save result in dictionary (globally available from service)
  * Every possibility of calls (so all pieces) been created on the beggining, so whenever user call the server the response going to be server immedietally
* The server was tested on files:
  * Max 8MB
  * Min 7B
* Empty response if piece is not existed or hash is incorrect `{"content":null,"proof":null}`
* Instead of SHA1 we are using SHA256
* Because I need a way to test `piece` result I decided to write own `Validate` method in `TreeManager` which I'm used in tests to confirm that all my logic is correct (back and forth)

## Future Code improvements (ideas)

* Expose `Validate` method, so user can validate some piece of file with proofs
* This is PoC, so for larger files or lot's of files, better idea is to calculate MerkleTree on demand and store result (similar behaviour to `AddSingleton` in ASP, create only when needs)
* Different implementation of MerkleTree, like support BigEndian/LittleEndian text

## Server settings

* Server was written with .NET Core 3.1 (https://dotnet.microsoft.com/en-us/download/dotnet/3.1)
* In the `appsettings.json` there is setup for the FileSize (default 1024KB but could be easily increased)

## Running

* You can run using VisualStudio (ex. 2022) with command line arguments (please recheck in `launchSettings.json` the correct port, the IISExpress is 10961)

![image](https://user-images.githubusercontent.com/5141943/206875711-4850184f-93c2-4c2d-955d-a71ead88bd53.png)

* You can use command line and type `dotnet run StoredFiles/icons_rgb_circle.png` (if you use command line the port is 5000)

![image](https://user-images.githubusercontent.com/5141943/206875778-f2370d53-e52d-4b88-9150-e97b564057df.png)

* The entire server also support multiple files, to run server with multiple files please separate files with coma: `dotnet run "StoredFiles/icons_rgb_circle.png;StoredFiles/test.png;StoredFiles/test.txt"`

All files used for this server are located in the `StoredFiles`.
For command line runup please go to the root directory (`appsettings.json` location)

## Tests



## Available Endpoints

### GET /hashes

This endpoint return a json list of the merkle hashes and number of 1KB pieces of the files this server is serving

```sh 
curl -i -H "Accept: application/json" -H "Content-Type: application/json" -X GET http://localhost:5000/hashes
```

```json
[{
  "hash": "9b39e1edb4858f7a3424d5a3d0c4579332640e58e101c29f99314a12329fc60b",
  "pieces": 17
}]
```

![image](https://user-images.githubusercontent.com/5141943/206875983-5944e62c-3d73-4526-ae47-cf52ee69befa.png)

### GET /piece/:hashId/:pieceIndex

This endpoint return a verifiable piece of the content.

Parameter   | Description
----------- | -------------
:hashId     | the merkle hash of the file we want to download (in our case there will only be one)
:pieceIndex | the index of the piece we want to download (from zero to 'filesize divided by 1KB')

The returned object will contain two fields:

Field   | Description
------- | -------------
content | The binary content of the piece encoded in base64.
proof   | A list of hashes hex encoded to prove that the piece is legitimate. The first hash will be the hash of the sibling and the next will be the uncle's hash, the next the uncle of the uncle's hash and so on.

Example:
```sh
curl -i -H "Accept: application/json" -H "Content-Type: application/json" -X GET http://localhost:5000/piece/9b39e1edb4858f7a3424d5a3d0c4579332640e58e101c29f99314a12329fc60b/8
```

```json
{
  "content": "1wSDXYz+dPEXQP9oAYKE7Tz5ttGgCYkD3ile/OXpP4AAAPTqv+BlsRiHgknDtgQv/orRny7+AhAAgB7a+tKLxbYEp8bkJiY7bdm/L7n35ek/QN/NOQMAGYi+8c17X7AQLf8MUxOjP83+B+jzn71XLs+ZAgQZiKkxO7QCtffz27kjyYu/zP0HGAwtQJCJGA36zFtvWIgWSrWF646n/wACAFBzIfnqL7qTZGiXFC/+uvPpZ0Z/AvTfpW4AmL9yedpaQD5iKpDRoO0q/lMc/an9B2Ag5roBwDpAXuI8wLOnTlqItgSABEd/xsVf97/+xocLMCACAGQoxkkaDdqGz2m8e3YjNXc+1fsPMCDfLQ0As9YD8vLM735jNGjDpdj7Hxd/Gf0JMDAzSwOAUaCQoWdPn3QeoKHi4i+jPwGogxYgyPkPgOefK3YYDdpITyfYouXiL4CBe6AFyG3AkKl4ymw0aLPErkyK7T93P/+imL923QcMMDhagIAFMRo0Wk5ohij+Y1pTam5+OOXDBWgALUBAt9jcaTRoY6Q4oenu+S9d/AUweHNLA8C09YC8xbjJ7a93LMSARTuWi78AqMP8lcsPBACAYvuvj3VnzzM42xK8+CtGf9676KgZQFMsBoA5SwGEnSffNhp0QOJehrikLTV6/wEa4cIDAWBxOwAg2k/iUDD9t83oTwD68b/1S/71VcsBhK0vvZjkGMomi10XF38BUKO5lQKABk3gB8+89Ua3JYX+SPHpf7jj6T9AowMAwA9iNOgOrUACwEaK/08/M/oToDm+WykATFsXYKkYDRo7AdQr1Yu/tP8ANMrMSgEA4CHbXv2F0aB1B4AER3/euzhb3P/6Gx8uQHOsuAPgDACwomdPnzQatCYRrmKnJTV3PtX7D9Ak81cur7gD8J2lAVYS7SnPnjppIWqQYu9/XPxl9CdAc9kBAFYlLqhKdVLNoDz10590R66mRu8/QONcWDEAzF+5bAcAeKxnfvcbo0F76GkXfwEwAMsPAc9aEuBxYjSo8wAbF2uY4mVrdz//opi/dt0HDNAs00v/j83L/p92AViXexdniv+z76VsC7mRf/mnYuj557J4v3FgdcdbbxTX3nnPF38DUh39efPDKR8uQPM8UOMPPS4dAE8WTzu/f/NEVu95+PDLxdZDB3z4G5DieYq757908RdAM808LgDYAYB1iHnnNz86k9V73vn7t7uHWFm7CE8p7hg5/AvQWHOPCwAmAcE6RetDXH6Ui2hfifMArN22V9Mc/RmtgAA0z/yVy48NAHOWCNbv+jvvdaeg5CJGg25/veODX4OYohTrlmIABqCRHno6OfS4dACsTfQ/53Y4dvuvj3Vvs2V1thn9CQ==",
  "proof": [
    "6a10a0b8c1bd3651cba6e5604b31df595e965be137650d296c05afc1084cfe1f",
    "956bf86d100b2f49a8d057ebafa85b8db89a0f19d5627a1226fea1cb3e23d3f3",
    "04284ddea22b003e6098e7dd1a421a565380d11530a35f2e711a8dd2b9b5e7f8",
    "c66a821b749e0576e54b89dbac8f71211a508f7916e3d6235900372bed6c6c22",
    "a8bd48117723dee92524c25730f9e08e5d47e78c87d17edb344d4070389d049e"
  ]
}
```

![image](https://user-images.githubusercontent.com/5141943/206876005-d6347d38-19f0-4363-9184-fe8b417c86da.png)
