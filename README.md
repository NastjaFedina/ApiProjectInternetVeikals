# ApiProjectInternetVeikals
ApiProjectInternetVeikals ir tiešsaistes veikala tīmekļa lietojumprogramma, kas tiek veidota, izmantojot ASP.NET, Entity Framework un REST. Tā arī ietver API un frontend, un demonstrē, kā izveidot backend sistēmu. Projekts ievieš JWT autentifikāciju, kas nodrošina aizsardzību aizsargātiem API resursiem. Pēc autorizācijas lietotājam tiek izsniegta token, kuru serveris pārbauda katram pieprasījumam. Kategorijas un pasūtījumi ļauj lietotājiem droši pārvaldīt datus un mijiedarboties ar sistēmu, demonstrējot pilnīgu REST API un klienta saskarnes integrāciju.

Satur:

*Controllers:
produktu apstrāde,
lietotāju pieprasījumi,
pasūtījumi,
autorizācija (login).

*Modeļi (tabulas): 
Product (pārstāv preci veikalā), 
Category (satur informāciju par produktu kategorijām), 
User (saglabā API lietotāja datus), 
Order (norāda lietotāja veiktu pasūtījumu), 
OrderItem (Saista produktu ar pasūtījumu).

*Foreign Keys:
Product.CategoryId: 
Category
Order.UserId: 
User
OrderItem.OrderId:
Order
OrderItem.ProductId: 
Product

*GET, POST metodes:
GET (atgriež visu produktu sarakstu no tabulas Products, atgriež pasūtījumu sarakstu, ielādē saistītos datus (User, OrderItems), ...)
POST (izveido jaunu pasūtījumu, saglabā to datubāzē, ...)
