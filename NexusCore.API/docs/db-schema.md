Users:
 - Id
 - Username
 - Email
 - Password (hashed)
 - Role
 - IsVerified
 - VerifyCode
 - CodeExpiry

Products:
 - Id
 - Name
 - Description
 - Price
 - StockQuantity
 - DiscountPercentage
 - PurchasePrice

Orders:
 - Id
 - UserId (foreign key with Users[id])
 - OrderDate
 - TotalAmount
 - Status

OrderItems:
 - Id
 - OrderId
 - ProductId
 - Quantity
 - Price