Задание от СПИДБОКС.
"
Есть API СДЭК https://api-docs.cdek.ru/29923741.html .
Требуется написать web api в котором будет один метод по расчёту стоиомсти доставки. Расчёт происзводить с использованием методв "Расчет стоимости по тарифам без приоритета" из документации СДЭК.
Метод будет принимать вес в граммах, габариты в мм, фиас код города отправления, фиас код города получения. На выходе должен стоимости доставки с помощью транспортной компании СДЭК груза. Рассматриваем лишь одноместные отправления и курьерскую доставку.
"

Важно.

Docker-файл находится не в корневой. Для правильной сборки image нужно указывать путь к файлу. Из корневой папки проекта `docker build -f ./ShippingCalculator.WebApi/Dockerfile .`

Для используемых запросов к API SDEC необходима авторизация через токен. У тестовых токенов короткий срок службы. Токен указывать в appsettings.json, свойство "Token". Запросить свежий токен можно, например, так `curl --location --request POST 'https://api.edu.cdek.ru/v2/oauth/token?grant_type=client_credentials&client_id=EMscd6r9JnFiQ3bLoyjJY6eM78JrJceI&client_secret=PjLZkKBHEiLK3YsjtNrt3TGNG0ahs3kG'`
