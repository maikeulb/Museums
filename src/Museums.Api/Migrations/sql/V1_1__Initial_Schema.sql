CREATE TABLE museums (
    "Id" serial PRIMARY KEY,
    "Name" varchar(50) NULL
);

CREATE TABLE paintings (
    "Id" serial PRIMARY KEY,
    "Title" varchar(50) NULL,
    "Artist" varchar(50) NULL,
    "Medium" varchar(50) NULL,
    "MuseumId" integer NULL
);

ALTER TABLE paintings
ADD CONSTRAINT fk_paintings_museums
FOREIGN KEY ("MuseumId")
REFERENCES museums("Id")
ON DELETE CASCADE;
