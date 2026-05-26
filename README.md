<img width="1920" height="340" alt="Comp 1_00000" src="https://github.com/user-attachments/assets/f553bb89-5e84-489d-b895-450864c98d8c" />
<p align="center">
<img src="https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white">
<img src="https://img.shields.io/badge/tailwindcss-%2338B2AC.svg?style=for-the-badge&logo=tailwind-css&logoColor=white">
<img src="https://img.shields.io/badge/vite-%23646CFF.svg?style=for-the-badge&logo=vite&logoColor=white">
<img src="https://img.shields.io/badge/docker-%230db7ed.svg?style=for-the-badge&logo=docker&logoColor=white">
<img src="https://img.shields.io/badge/markdown-%23000000.svg?style=for-the-badge&logo=markdown&logoColor=white">
<img src="https://img.shields.io/badge/OpenTelemetry-F5B727?&style=for-the-badge&logo=opentelemetry&logoColor=black">
</p>

### <b>A document management service designed to enable strict access control, auditing and rapid identification of data breach sources.</b>
<br>

> [!CAUTION]
> <b>This project is a work in progress and is yet to undergo rigorous security and general testing.</b>

## Roadmap
- [X] Base features like account, document and group creation.
- [X] Responsive pages.
- [X] Account deletion, password change 
- [X] Digital signing with ECDSA.
- [X] AES-256 bit cold storage encryption.
- [x] Admin accounts
- [x] Access periods ("allow access for X days").
- [x] Read only PDF view.
- [X] Tracking information in read only mode, making each download linked to the IP and account of the user.
- [X] Event logs.
- [ ] Captcha.
- [ ] Customizable theme and front page.

<br>

## Brief technical details
- All changes are signed with the private key of the user who made them using ECDSA. In the future, this private key will be encrypted such that it is only decryptable with the user's password, which the server does not know directly.
- If you enable document encryption, it means all the document's data is encrypted using AES-256 during rest and only briefly decrypted server side using the user's provided key, when a user modifies or tries to read the contents of the document.

## Installation (requires .NET 10 or Docker)
```
git clone https://github.com/varppi/ConfiDoc
cd ConfiDoc/Confidoc.Server
dotnet ef database update
dotnet run .
```
or use the dockerfile included to fit your custom needs.

## Configuration
<b>Location: `Confidoc.Server/.env`</b>

Sample .env (sample contains all currently configurable settings)
```
CONFIDOC_DATABASE="sqlite"
CONFIDOC_CONNECTION="Data Source=confidoc.db"
CONFIDOC_JWT_SECRET="testingtestingTesting1234!Teeeestinng!"
CONFIDOC_JWT_ISSUER="https://localhost:5173"
CONFIDOC_JWT_AUDIENCE="https://localhost:5173"
CONFIDOC_JWT_EXPIRES=60 # minutes
PASSWORD_REQUIRE_DIGITS=true
PASSWORD_REQUIRE_NONALPHA=true
PASSWORD_REQUIRE_UPPER=true
PASSWORD_REQUIRE_LOWER=true
LOG_TYPE=console
LOG_OUT=null # file/elasticsearch/other supported destiations for serilog to output logs
LOG_LEVEL=debug
```

## Screenshot gallery
### Front page
<img width="1917" height="906" alt="image" src="https://github.com/user-attachments/assets/c542b294-c27c-4b20-92cd-4bdd86da8124" />

### Event logs show 50 of the latest document related events (admin sees every document, normally you see only documents you own). Different actions have different colored text to make it easier to spot unusual activity.
<img width="1919" height="943" alt="image" src="https://github.com/user-attachments/assets/35439e45-4edf-4f13-9830-07d689afecd7" />


### Markdown editor
<img width="1920" height="944" alt="image" src="https://github.com/user-attachments/assets/2d216428-7015-4945-bc4d-1fb44c479826" />

### Downloaded PDF version of the document. The vertical gray lines contain the download event ID in binary format, which can be used to query who downloaded the PDF file.
<img width="1915" height="941" alt="image" src="https://github.com/user-attachments/assets/14ce78b7-93be-4a26-8e69-a16ea8c43d26" />

