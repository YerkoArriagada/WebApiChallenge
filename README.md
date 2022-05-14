# WebApiChallenge

## I. Código fuente repositorio github

Enlace: https://github.com/YerkoArriagada/WebApiChallenge.git

## II. Instrucciones de cómo ejecutar el programa o levantar la app.

  Para poder levantar el proyecto necesitamos tener instalado Visual Studio Code, el cual lo pueden descargar del siguiente enlace: https://code.visualstudio.com/

1. Después de haber descargado el proyecto **WebApiChallenge** debemos abrir una terminal PowerShell o Command Prompt (CMD)

![Screenshot_1](https://user-images.githubusercontent.com/63363780/168411943-1276e679-a17d-4a94-8688-6af228c001de.png)

2. Una vez abierta la terminal debemos navegar hasta la carpeta del proyecto con el comando ***cd***. Luego de eso ejecutamos el comando ***code .*** lo que abrirá el proyecto en Visual Studio Code

![Screenshot_2](https://user-images.githubusercontent.com/63363780/168412059-f7ba63b0-40b6-4dfc-9723-6c6469b6952e.png)

3. Cuando se ejecuta Visual Strudio Code nos preguntara si ¿Confía en los autores de los archivos de esta carpeta?, le damos clic en ***Si, confío en los autores***

![Screenshot_3](https://user-images.githubusercontent.com/63363780/168412304-42018b75-0a8d-432d-8845-6b9b0858bc2c.png)

4. Abrimos una nueva terminal y ejecutamos el comando ***dotnet run***. También podemos observar la url y puerto (https://localhost:7030) donde podremos acceder al API REST.

![imagen](https://user-images.githubusercontent.com/63363780/168412409-23172513-13e1-4ca4-9649-84802cc2de00.png)

![imagen](https://user-images.githubusercontent.com/63363780/168412462-dd733a33-9b47-4587-a291-106e149fc045.png)

5. Podremos acceder de manera mas sencilla a los recursos de la API, debemos acceder en nuestro navegador a https://localhost:7030/swagger/index.html.

![Screenshot_6](https://user-images.githubusercontent.com/63363780/168412771-369c3fe8-d017-4cf6-bb0a-261fbe8a7710.png)

## Recursos

### Item

La api cuenta con una base de datos (Microsoft SQL Server Express LocalDB) basica precargada con Items y Usuarios.

***- Optener la lista de items:***

curl -X 'GET' 'https://webapichallenge20220514002445.azurewebsites.net/api/items' -H 'accept: text/plain'

***- Obtener un item por id:***

curl -X 'GET' 'https://webapichallenge20220514002445.azurewebsites.net/api/items/1' -H 'accept: text/plain'

***- Buscar item por item_id:***

curl -X 'GET' 'https://localhost:7030/api/items/MLA' -H 'accept: text/plain

***- Modificar un item por id:***

curl -X 'PUT' 'https://webapichallenge20220514002445.azurewebsites.net/api/items/1' -H 'accept: */*' -H 'Content-Type: application/json'

{
  "id_item": "string",
  "price": 0
}

***- Eliminar item por id:***

curl -X 'DELETE' 'https://localhost:7030/api/items/1' -H 'accept: */*'

***- Agregar un item:***

curl -X 'POST' 'https://localhost:7030/api/items/saveitem'  -H 'accept: */*' -H 'Content-Type: application/json' -d

{
  "id_item": "string",
  "price": 0
}

***- Agregar items de forma masiva***

curl -X 'POST' 'https://localhost:7030/api/items/saveitems' -H 'accept: */*' -H 'Content-Type: application/json' -d 
{
  "url": "https://api.mercadolibre.com/sites/MLC/search?q=Kits%20Cotillón"
}

### Usuarios

***- Optener la lista de usuarios:***

Curlcurl -X 'GET' 'https://localhost:7030/api/users'  -H 'accept: text/plain'

***- Obtener un usuario por id:***

curl -X 'GET' 'https://localhost:7030/api/users/1'  -H 'accept: text/plain'

***- Buscar usuario por nombre:***

curl -X 'GET' 'https://localhost:7030/api/users/name' -H 'accept: text/plain'

***- Modificar un usuario por id:***

curl -X 'PUT' 'https://localhost:7030/api/users/1' -H 'accept: */*' -H 'Content-Type: application/json' -d 
{
  "nombre": "string", 
  "itemsIds": [1, 2, 3]
}

***- Eliminar usuario por id:***

curl -X 'DELETE' 'https://localhost:7030/api/users/1' -H 'accept: */*'

***- Agregar un usuario:***

curl -X 'POST' 'https://localhost:7030/api/users/saveuser' -H 'accept: */*' -H 'Content-Type: application/json' -d 

{
  "nombre": "string", 
  "itemsIds": [1, 2, 3]
}

***- Agregar usuarios de forma masiva***

curl -X 'POST' 'https://localhost:7030/api/users/saveusers' -H 'accept: */*' -H 'Content-Type: application/json' -d 

[
  {
    "nombre": "string", 
    "itemsIds": [1, 2, 3]
  }
]

