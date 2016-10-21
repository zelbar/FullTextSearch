# Full Text Search
Solution to the [first project/laboratory assignment](Upute_za_1._projekt_-_pretrazivanje_teksta_i_napredni_SQL.pdf) at the [Advanced Databases](http://www.fer.unizg.hr/en/course/advdat) course.

## Technologies
- ASP.NET Core
- PostgreSQL

## Dataset
[Neural Information Processing Systems (NIPS) 2015 papers](https://www.kaggle.com/benhamner/nips-2015-papers)
### Papers.csv
This file contains one row for each of the 403 NIPS papers from this year's conference. It includes the following fields
- Id - unique identifier for the paper (equivalent to the one in NIPS's system)
- Title - title of the paper
- EventType - whether it's a poster, oral, or spotlight presentation
- PdfName - filename for the PDF document
- Abstract - text for the abstract (scraped from the NIPS website)
- PaperText - raw text from the PDF document (created using the tool pdftotext)

## Run instructions
Install PostgreSQL 9.5+ and ASP.NET Core 1.0.1 (Windows/Linux/Mac).
Run from shell these commands in the root of the repository
``dotnet restore
dotnet run``