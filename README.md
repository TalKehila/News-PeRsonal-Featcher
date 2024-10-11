THERE ARE SECRETS ENV THE PROJECT WILL NOT RUN WITHOUT IT TALK TO ME BEFORE !! THERE IS NO DOCKER COMPOSE !!! 

# project

News personal fetcher

## desc

A microservice app that aggregates news and tech updates, uses AI to select and summarize articles, and sends updates via email or Telegram.

Microservices project with 3 services: UserDbAccessor, NewsFetchSummarizeService, and AggregatorNotifaction.

UserDbAccessor: Manages user data, including creation and preferences.

NewsFetchSummarizeService: Fetches and summarizes news based on user preferences.

AggregatorNotifaction: Sends notifications and aggregates user data.

## How to run

1. git clone
   `git clone https://github.com/TalKehila/News-PeRsonal-Featcher.git`

2. cd to project dir
   `cd News-PeRsonal-Featcher`

3. run app
   `docker-compose up --build -d`

4. open browser

[AggregatorNotifaction](http://127.0.0.1:8082/swagger)

[UserDbAccessor](http://127.0.0.1:8080/swagger)

[NewsFetchSummarizeService](http://127.0.0.1:8081/swagger)

## How to check the app

## Best Way for Check

- Enter [UserDbAccessor](http://127.0.0.1:8080/swagger)
  and create a user (Follow the Create User topic) and the mail will send automaticlly (please user an email adress that you have access to)

## For checking manually each micro service.

- Before you start please do it in the next order of each micro service.

  UserDbAccessor > NewFeatchSummrizeService > AggregatorNotifaction

## Create USER

1. create new user in Post method and try it out btn in the
   [UserDbAccessor](http://127.0.0.1:8080/swagger)
   (please use an email adress that you have access to) for your convince use
   [temp-mail](https://temp-mail.org/).

   choose one topic from the list preference news would you like.
   THE LIST:
   business - crime - domestic - education - entertainment - food - health - lifestyle - other - politics -science - tourism - world

   in the communication channel please write Email.

   for Example- `{
"id": 0,
"fullName": "test",
"email": "Test@gmail.com",
"phone": "05044444444",
"preferences": "crime",
"communicationChannel": "Email"
}`

2. Checking service.
   [NewsFetchSummarizeService](http://127.0.0.1:8081/swagger)

   News Fetch Service - In the swagger can click on Try it out after with you get the user id and fill it in the text box and click execute and wait for fetching data.

3. Checking service aggregator notifaction.
   [AggregatorNotifaction](http://127.0.0.1:8082/swagger)
   Enter to the swagger and Try it out wait a few secound and check you email
   (should get a mail from news aggregator with you news preferences).
   =======

# News-Personal-Fetcher-

Personal new featcher

> > > > > > > # 2595a1895a457d22e7cafb630c30c0e7444ca392

# Personal-News-fetcher

project

> > > > > > > origin/main
