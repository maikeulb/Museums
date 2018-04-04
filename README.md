# Museums

Restful API for NYC Museums. The resources are the museums and their paintings.

Technology
----------
* Suave
* PostgreSQL (using SQLProvider)
* Flyway

Endpoints
---------

| Method     | URI                                  | Action                                 |
|------------|--------------------------------------|----------------------------------------|
| `GET`      | `/api/museums`                       | `Retrieve all museums`                 |
| `GET`      | `/api/museums/{mid}`                 | `Retrieve museum`                      |
| `POST`     | `/api/museums`                       | `Add museum`                           |
| `PUT`      | `/api/museums/{mid}`                 | `Update museum`                        |
| `DELETE`   | `/api/museums/{mid}`                 | `Remove museum`                        |
| `GET`      | `/api/museums/{mid}/paintings`       | `Retrieve all museum paintings`        |
| `GET`      | `/api/museums/{mid}/paintings/{id}`  | `Retrieve museum painting`             |
| `POST`     | `/api/museums/{mid}/paintings`       | `Add museum painting`                  |
| `PUT`      | `/api/museums/{mid}/paintings/{id}`  | `Update museum painting`               |
| `DELETE`   | `/api/museums/{mid}/paintings/{id}`  | `Delete museum painting`               |

Sample Responses
---------------
`http get http://localhost:8080/api/museums` 
```
[
    {
        "id": 1, 
        "name": "Museum of Modern Art"
    }, 
    {
        "id": 2, 
        "name": "Whitney Museum of American Art"
    }, 

...
```
`http get http://localhost:8080/api/museums/{mid}/paintings` 
```
[
    {
        "artist": "Vincent van Gogh", 
        "id": 1, 
        "medium": "Oil on canvas", 
        "museumId": 1, 
        "title": "The Starry Night"
    }, 
    {
        "artist": "Salvador Dali", 
        "id": 2, 
        "medium": "Oil on canvas", 
        "museumId": 1, 
        "title": "The Persistence of Memory"
    }, 
...
```
Run
---

You need Mono, forge, and fake to run the API. There are two branches: the
`master` branch is the in-memory solution (which is buggy), and the `postgres`
branch is the persistent solution. To run the `postgres` branch: create
a database, open `db.fs` and point the URI to your database, and apply the
migrations (scripts located in the migrations folder).

```
git checkout postgres
forge fake run
Go to http://localhost:8080 and visit one of the above endpoints
```

TODO
----
Dockerfile  
