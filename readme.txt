+++ DATABASE SCHEMA +++

Warning: the schema contains only few tables and is not normalized as it is not in the focus of this project.

=== table_name
|___	column_name	: type		(constraints, indices)

===	paper
|___	id 			: integer 	(PK)
|___	title 		: text 		(titlelowertrgmidx:gist)
|___	eventtype 	: text
|___	pdfname 	: text
|___	abstract	: text		(abstractlowertrgmidx:gist)
|___	papertext	: text
|___	tsv			: tsvector	(tsvidx:gin, trgmidx:gist)

===	search
|___	query			: text
|___	type			: char[]
|___	operator		: char[2]
|___	numberofresults	: integer
|___	page			: integer
|___	querytime		: long
|___	timestamp		: timestamp_without_timezone

=== month
|___	id		: char[3]
|___	name	: text
|___	number	: integer

=== hour
|___	number	: integer
|___	text	: text

+++ TECHNOLOGIES +++

Web back-end: ASP.NET Core 1.0 using Dapper ORM, developed on Windows10 with Visual Studio, deployed to Linux (Ubuntu) at http://fts.željko.hr (via CloudFlare).
Web front-end: Twitter Bootstrap, jQuery, DateTimePicker library

+++ HOW TO RUN +++

Install PostgreSQL 9.5+ at port 5432 with user 'fts' (password 'fts') as superuser for database 'fts' (or change the ConnectionString in /src/FullTextSearch/appsettings.json if different). Restore the fts.backup database backup/dump file into the fts database.

Install .NET Core (Windows, Linux, Mac): https://www.microsoft.com/net/core

Run 'git clone https://github.com/zelbar/FullTextSearch.git'.

Position yourself at /src/FullTextSearch and run commands 'dotnet restore' and then 'dotnet run'. Application will start listening at 0.0.0.0:7923. Setup desired reverse-proxy server (eg. nginx).
