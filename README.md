> [!CAUTION]
> <b>This project is a work in progress. Not all the features advertised on the front page are implemented yet and the project has yet to undergo rigorous security testing.</b>

# ConfiDoc
<b>A document management service designed to keep track of changes and movement of data like a hawk.</b>

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
CONFIDOC_JWT_EXPIRES=60
PASSWORD_REQUIRE_DIGITS=true
PASSWORD_REQUIRE_NONALPHA=true
PASSWORD_REQUIRE_UPPER=true
PASSWORD_REQUIRE_LOWER=true
LOG_TYPE=console
LOG_OUT=null
LOG_LEVEL=debug
```

## Screenshot gallery
<img width="500" src="https://github.com/user-attachments/assets/382d339b-81f6-4a70-b11e-4739e2ad235d" />
<img width="500" src="https://github.com/user-attachments/assets/6a4786ed-bae5-4fab-891e-6e8805c4409d" />
<img width="500" src="https://github.com/user-attachments/assets/dbf085c2-6a79-4530-8379-6b01ea2d16fb" />

