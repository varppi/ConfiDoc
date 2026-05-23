<img width="1920" height="340"  src="https://github.com/user-attachments/assets/36f26726-64f5-41c9-8136-e9261dbb6ac3" />
<p align="center">
<img src="https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white">
<img src="https://img.shields.io/badge/tailwindcss-%2338B2AC.svg?style=for-the-badge&logo=tailwind-css&logoColor=white">
<img src="https://img.shields.io/badge/vite-%23646CFF.svg?style=for-the-badge&logo=vite&logoColor=white">
<img src="https://img.shields.io/badge/docker-%230db7ed.svg?style=for-the-badge&logo=docker&logoColor=white">
<img src="https://img.shields.io/badge/markdown-%23000000.svg?style=for-the-badge&logo=markdown&logoColor=white">
<img src="https://img.shields.io/badge/OpenTelemetry-F5B727?&style=for-the-badge&logo=opentelemetry&logoColor=black">
</p>

### <b>A document management service designed to aid secure document handling, to streamline access control and make it easier to discover sources of data leaks.</b>
<br>

> [!CAUTION]
> <b>This project is VERY work in progress. Not all the features advertised on the front page are implemented yet and the project has yet to undergo rigorous security testing.</b>

## Roadmap
- [X] Base features like account, document and group creation.
- [X] Responsive pages.
- [X] Account deletion, password change 
- [X] Digital signing with ECDSA.
- [X] AES-256 bit cold storage encryption.
- [x] Admin accounts
- [x] Access periods ("allow access for X days").
- [ ] Read only PDF view.
- [ ] Tracking information in read only mode, making each download linked to the IP and account of the user.
- [ ] Captcha.
- [ ] Customizable theme and front page.

<br>

## Brief technical details
- All changes are signed with the private key of the user who made them using ECDSA. In the future, this private key will be encrypted such that it is only decryptable with the user's password, which the server does not know.
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
<img width="1200" src="https://github.com/user-attachments/assets/382d339b-81f6-4a70-b11e-4739e2ad235d" />
<img width="1200" src="https://github.com/user-attachments/assets/6a4786ed-bae5-4fab-891e-6e8805c4409d" />
<img width="1200" src="https://github.com/user-attachments/assets/dbf085c2-6a79-4530-8379-6b01ea2d16fb" />

