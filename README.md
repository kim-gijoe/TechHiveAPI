# UserManagementAPI

A lightweight ASP.NET Core Minimal API for managing users, built as the final project for the **Back-End Development with .NET** Coursera course.  
The project demonstrates core back-end development skills including API design, validation, middleware implementation, authentication, logging, and error handling.

---

## Features

- Full CRUD operations for user management  
- In-memory storage (no external database required)  
- Input validation (required name, age > 0)  
- Centralized error-handling middleware returning consistent JSON errors  
- Custom authentication middleware using a bearer token  
- Logging middleware for auditing requests and responses  
- Pagination support for improved performance  
- Swagger UI for interactive API exploration  
- REST Client `.http` test suite included

---

## Endpoints

- `GET /users`  
- `GET /users/{id}`  
- `POST /users`  
- `PUT /users/{id}`  
- `DELETE /users/{id}`  

---

## Validation Rules

- `UserName` must not be empty or whitespace  
- `Age` must be greater than 0  
- Updating or deleting a non-existent user returns `404 Not Found`

---

## Middleware

### Error Handling
- Catches unhandled exceptions  
- Logs errors  
- Returns JSON:  
  `{ "error": "Internal server error." }`  
- Ensures consistent responses across all endpoints

### Authentication
- Requires a valid bearer token in the `Authorization` header  
- Invalid or missing tokens return `401 Unauthorized`

### Logging
- Logs HTTP method, request path, and response status code  
- Useful for auditing and debugging

---

## Performance

The `GET /users` endpoint supports pagination:

/users?skip=0&take=50


This prevents returning unnecessarily large datasets and improves responsiveness.

---

## Testing

Use the included `.http` file with the VS Code REST Client extension to test:

- Valid and invalid requests  
- Authentication behavior  
- Error handling  
- Pagination  
- CRUD operations  
- Middleware responses  

---

## Running the API

dotnet run


---

## Swagger UI

Once running, open:

http://localhost:5000/swagger