Coding:

| Symbol | Description |
|--------|-------------|
| ? | Respresents a request
| ! | Represents a answer
| . | Represents an assertion

Setup:

| Req | Description |
|-----|-------------|
| ?GAMEDATA | Request all server info regarding static
| !GAMEDATA | Answer
| ?ENTITIES | Request all clients info to sync with server state
| !ENTITIES | Answer
| .HANDSHAKE | Tells other the sender is ready for final sync

Game:

| Req | Description |
|-----|-------------|
| .NEWPLAYER   | Tells client the server has a new player
| .RMPLAYER   | Tells client the server has lost a player
| .PLAYER   | Send heartbeat with position/orientation
| .RMENTITY | Tell server to remove an entity from world space
| .SPWNENTITY | Tell server to spawn an entity in world space