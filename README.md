# Dreamer Web API Task.

Restful API Client for **Ordering System** with products, categories and orders in addition to basic login (JWT tokens) and logger system.

## Project Components

The project has been made using DDD pattern, which has those folders:
* BoundedContext which contains the domains (repos), dtos and SharedContext.
* Infrastructure which contains the EF context and its models, migrations, and tools.
* Services which contains 3 Services:
    * EmailNotifier which is responsible for sending notifications after the order has been paid, shipped, or delivered.
    * InternalHangfire which is responsible to enqueue (using hangfire library) the shipping in 24 hours after the order has been payed.
    * ShippingCostCalculator which is just a *fake* service, suppose to calculate the delivery costs.
* WebApi which represents the endpoint, and it also contains *appsettings* file where you can find the connection strings, Email config, and token config.

## Libraries I used:
Database:
```
Entityframework core
```

Logging framework:
```
Serilog
```

Background jobs:
```
Hangfire
```

API actions documentation:
```
Swagger
```

## Running the project

## Few words about how it works:
### First-run:
After running the project project for the first time, it will use the default sql server connection (local) to create two databases with names "Dreamer" and "Dreamer.Hangfire". One is for storing orders data and the other is for storing jobs (enqueued methods) which belongs to hangfire service.

### API Actions Snapshot:
After running the project, you can simply go to swagger index page ([/swagger](http://localhost:53057/swagger)), now you can see all the actions of the API and even try them.

### Seeding:
As you can notice in the swagger index, you can seed the data by requesting the url ([/Seed/SeedDatabase/](http://localhost:53057/Seed/SeedDatabase)) [GET-Anonymous]
And it will clear the old data/seeds so you'll have fresh data with random names/numbers except for the users which have constant info (as below).

**Default users info:**

| Email | Password | Role |
| --- | --- | --- |
| admin@dreamer.com | admin | Administrator |
| customer@dreamer.com | customer | Customer |

*Note: it's better to change user email or add new user (see below) in order to recieve valid emails about changing order status (paid, shipped, and delivered)*
### Where Am I:
Simply, you should identify yourself by requesting a token using **TokenController**, providing your information. Then you can use all other actions as a logged-in user.
You can get all available products from **Products Controller** with optional filters (name/category) in addition to paging-related parameters (pageSize/pageIndex), and add the ones you want to your cart using **Carts Controller** which allows you to add/edit/remove products from your cart, 

You can check your cart items using *GetMyCartItems* action, and once all set, you can checkout your items using *CheckOutCart* which also will move all your cart to new *Paid* order which will return your order details and will email you about the details.

The order status will be change to shipped in 24 hours using Hangfire *background jobs*, and you will be notified with an email.
The admin can change the order status to delivered using **AdminController**, and once invoked, you'll be notified too.
To check your order status, you can always request *GetMyOrders* in **CartController**.

The admin also can check all users orders status in addition to adding new users and setting orders as delivered.


## Validation:
I added some sort of basic DTOs validation, all manipulated and invalid request will return *[BadRequest 400]* either with or without explanation message.
There could be added some more validation layers, but that will cost much more time and I don't think that these are that important, I already handled all kind of possible posted values. but the error message isn't always helpful.

## Documentation:
I tried to explain add some information about the repositories methods (the interface not the class) and also whenever I thought that there should be an explanation (some API actions, some Models properties, some dtos properties), I think over-documenting will make the whole think useless.
So I just added comments wherever I thought it's *required*.

## What about the logger:
The logs will be written to a file inside inside *D partition* (*dreamerLogs.txt*), and I put it there duo to permissions issue.
All non-handled exceptions will be written, which will allow the developer to track and trace exceptions.

*Note: the minimum log event level is Information.*
## Using the API:
You can use those cURL with any app (Insomnia/Postman) :
#### Get Token
```
curl --request POST \
  --url http://localhost:53057/Token/GetToken \
  --header 'content-type: application/json' \
  --data '{
	"Email":"admin@dreamer.com",
	"Password" : "admin"
}'
```
#### Note that you have to replace the *token* and other parameters (Ids) in order to test those requests.
***
#### Get Available Products (with optional filters)
You can leave all the data body empty, the default *pageIndex is 1* and the default *pageSize is 20*
```
curl --request POST \
  --url http://localhost:53057/Product/GetProducts \
  --header 'authorization: Bearer [token]' \
  --header 'content-type: application/json' \
  --data '{
	"PageIndex" :1,
	"PageSize" : 25,
	"Name" : "",
	"CategoryId" : ""
}'
```

#### Add Product To My Cart
```
curl --request PUT \
  --url http://localhost:53057/Cart/AddProductToCart \
  --header 'authorization: Bearer [token]' \
  --header 'content-type: application/json' \
  --data '{
	"ProductId" : "d18c1cad-f862-46a5-12f8-08d69cd3dcd1",
	"Quantity" : 2
}'
```
#### Edit Cart Item Quantity
```
curl --request PUT \
  --url http://localhost:53057/Cart/EditCartItemQuantity \
  --header 'authorization: Bearer [token]' \
  --header 'content-type: application/json' \
  --data '{
	"ProductId" : "2793b78c-872b-4ddf-12f6-08d69cd3dcd1",
	"Quantity" : 10
}'
```
#### Delete Product From My Cart
```
curl --request DELETE \
  --url http://localhost:53057/Cart/DeleteProductFromCart \
  --header 'authorization: Bearer [token]' \
  --header 'content-type: application/json' \
  --data '{
	"ProductId" : "e6fff6e1-5f5b-4a84-12f7-08d69cd3dcd1"
}'
```
#### Get My Current Cart Items
```
curl --request POST \
  --url http://localhost:53057/Cart/GetMyCartItems \
  --header 'authorization: Bearer [token]'
```
#### Check Out Cart
```
curl --request PUT \
  --url http://localhost:53057/Cart/CheckOutCart \
  --header 'authorization: Bearer [token]' \
  --header 'content-type: application/json' \
  --data '{
	"Address" : "My shipping address"
}'
```
#### Get Order Status
```
curl --request POST \
  --url http://localhost:53057/Cart/GetOrderStatus \
  --header 'authorization: Bearer [token]' \
  --header 'content-type: application/json' \
  --data '{
	"OrderId" : "643305d7-9359-4fd4-cb5e-08d69cf9ff99"
}'
```
#### Get My Orders
Just like the previous one, but it gets all orders.
```
curl --request POST \
  --url http://localhost:53057/Cart/GetMyOrders \
  --header 'authorization: Bearer [token]'
```
#### Clear My Cart
```
curl --request DELETE \
  --url http://localhost:53057/Cart/ClearCart \
  --header 'authorization: Bearer [token]'
```

## Admin Actions
#### Get Specefic User Orders
```
curl --request POST \
  --url http://localhost:53057/Admin/GetUserOrders \
  --header 'authorization: Bearer [token]' \
  --header 'content-type: application/json' \
  --data '{
	"UserId" : "8e06cae2-0f3b-4f84-9b5b-08d69c915691"
}'
```

#### Mark Order As Delivered
```
curl --request PUT \
  --url http://localhost:53057/Admin/MarkOrderAsDelivered \
  --header 'authorization: Bearer [token]' \
  --header 'content-type: application/json' \
  --data '{
	"OrderId" : "643305d7-9359-4fd4-cb5e-08d69cf9ff99"
}'
```

#### Add New User
```
curl --request PUT \
  --url http://localhost:53057/Admin/AddNewUser \
  --header 'authorization: Bearer [token]' \
  --header 'content-type: application/json' \
  --data '{
	"Email" : "abdulmalekalbakkar@gmail.com",
	"Name" : "Mallok",
	"Password" : "123",
	"Role" : 1
}'
```
