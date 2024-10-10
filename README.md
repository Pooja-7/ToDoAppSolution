Here is a refined version of your README with proper Markdown syntax:

# ToDo API

## Setup

### Prerequisites:
- **.NET SDK** (version 8 or higher)
- **A SQL database** (e.g., SQL Server) for Entity Framework Core
- **An IDE** like Visual Studio or Visual Studio Code

### Clone the Repository:
```bash
git clone <repository-url>
cd <repository-name>
```

### Install Dependencies:
Navigate to the project folder and run:

```bash
dotnet restore
```

### Configure Database Connection:
Update the `appsettings.json` file with your database connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<server>;Database=<database>;User Id=<username>;Password=<password>;"
  }
}
```

### Run Migrations:
Apply migrations to create the database schema:

```bash
dotnet ef database update
```

### Run the Application:
```bash
dotnet run
```

The API will be available at `http://localhost:<port>/api/ToDo`.

---

## Services Overview

### 1. Create ToDo Item
- **Endpoint**: `POST /api/ToDo/create`
- **Request Body**:
  ```json
  {
    "title": "Sample ToDo",
    "isCompleted": false,
    "userId": "05e4763c-bb2b-46b3-a5d1-1b25351faeeb"
  }
  ```
- **Response**:
  - **Success (200 OK)**:
    ```json
    {
      "success": true,
      "data": {
        "id": 1,
        "title": "Sample ToDo",
        "isCompleted": false,
        "userId": "05e4763c-bb2b-46b3-a5d1-1b25351faeeb"
      }
    }
    ```
  - **Error (400 Bad Request)**:
    ```json
    {
      "success": false,
      "errorNumber": 303,
      "errorDescription": "Failed to create ToDo item. <error message>"
    }
    ```

### 2. Update ToDo Item
- **Endpoint**: `PUT /api/ToDo/update/{userId}/{id}`
- **Request Body**:
  ```json
  {
    "title": "Updated ToDo",
    "isCompleted": true
  }
  ```
- **Response**:
  - **Success (204 No Content)**:
    ```json
    {
      "success": true,
      "data": true
    }
    ```
  - **Error (404 Not Found)**:
    ```json
    {
      "success": false,
      "errorNumber": 301,
      "errorDescription": "ToDo item not found."
    }
    ```
  - **Error (403 Forbidden)**:
    ```json
    {
      "success": false,
      "errorNumber": 302,
      "errorDescription": "User ID does not match. Cannot update ToDo item."
    }
    ```

### 3. Delete ToDo Item
- **Endpoint**: `DELETE /api/ToDo/delete/{userId}/{id}`
- **Response**:
  - **Success (204 No Content)**
  - **Error (404 Not Found)**:
    ```json
    {
      "success": false,
      "errorNumber": 307,
      "errorDescription": "ToDo item not found."
    }
    ```

### 4. Get All ToDo Items
- **Endpoint**: `GET /api/ToDo/getAll`
- **Response**:
  - **Success (200 OK)**:
    ```json
    {
      "success": true,
      "data": [
        {
          "id": 1,
          "title": "Sample ToDo",
          "isCompleted": false,
          "userId": "05e4763c-bb2b-46b3-a5d1-1b25351faeeb"
        }
      ]
    }
    ```
  - **Error (500 Internal Server Error)**:
    ```json
    {
      "success": false,
      "errorNumber": 300,
      "errorDescription": "Failed to retrieve ToDo items. <error message>"
    }
    ```

### 5. Get All ToDo Items by User ID
- **Endpoint**: `GET /api/ToDo/getAllByUserId/{userId}`
- **Response**:
  - **Success (200 OK)**:
    ```json
    {
      "success": true,
      "data": [
        {
          "id": 1,
          "title": "User's ToDo",
          "isCompleted": false,
          "userId": "05e4763c-bb2b-46b3-a5d1-1b25351faeeb"
        }
      ]
    }
    ```
  - **Error (404 Not Found)**:
    ```json
    {
      "success": false,
      "errorNumber": 301,
      "errorDescription": "ToDo item not found."
    }
    ```

---

## User ID Relation

### Why is `userId` Used?

1. **User Identification**: The `userId` parameter is critical in ensuring that each `ToDoItem` is associated with a specific user. This prevents users from accessing or modifying each other's tasks, thereby enforcing data privacy and integrity.

2. **Authorization**: By checking the `userId` during operations like creating, updating, or deleting `ToDoItems`, the API ensures that users can only perform actions on their own tasks. This is essential for maintaining security and user trust in the application.

3. **Data Organization**: Using `userId` allows the API to organize `ToDoItems` in a way that facilitates easy retrieval and management based on individual user accounts, improving user experience by providing personalized data.

---

## Conclusion
This README outlines the necessary setup instructions, service descriptions, and the critical role of `userId` within the API. Make sure to adhere to these specifications when developing or using the ToDo API. For any further questions, please refer to the API documentation or reach out for support.

---
