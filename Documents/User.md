# User stories

### Description of a regular  `User`

---

### **All features:**

- Registration/Login
- Create groups
- Group additions
- Creating debt
- Repayment of debts
- Debt Optimization
- View debts
- View balance
- View history

---

## Descriptions of Features

### **Registration/Login**

Registration based on `mail` and basic information about the user. Credential login as username and password.
_For registrations:_

- The user indicates the mail which he also confirms.
- The user specifies personal information such as First Name/Last Name/Display Name(username)/...(can be completed).

_For authorizations:_

- The user enters a username and a password.

---

### **Create groups**

Groups can be created by giving them a name and description. When creating a group, the user opens up the possibility of adding people there for further actions.

_To create a group:_

- The user must provide a name and description for the group.

_Additional :_

- Each group has its own history and general condition.
- The optimization is applied relative to the group.

---

### **Adding to a group**

After creating a group, the user can add users there. Other users can also add other users after adding them to the group.

_To add to a group:_

- The user must select the desired user and the desired group.
- Each added member of the group can also call others.

---

### **Creating debt**

Debt is created when a user lends money to another user. The other user must confirm that he has this debt. It should also be borne in mind that debts are created strictly within the group.

_To create debt:_

- The user must specify the amount of the debt
- The user must specify the currency of the debt
- The user must specify a recipient
- The user must specify a group
- The user must indicate the status of the debt (Hidden Open)

Debts during creations must be confirmed by both parties that the act of loan was made. When requesting general group information, hidden debts are not included in the calculations.

---

### **Repayment of debts**

With the help of this function, the user can pay off his debts in full or in part. Also, in case of incomplete repayment of the debt, the amount of the debt is reduced. All debt repayment operations work in the same way as the creation of debts by mutual agreement.

_For repayment of debt:_

- The user must specify a debt
- The user must specify the amount of the debt to be returned
- The user must specify the currency of the returned debt

---

### **Debt Optimization**

Debt optimization is available to a user within a specific group. Access optimization at the request of any of the users but by mutual agreement of all users in the group.

_To request optimizations:_

- The user must specify a group
- Get approval from everyone

_Additionally:_
The debt optimization algorithm will work in such a way that the end result will have a minimum number of debtors and debts to make it easier for users to conduct financial relationships with each other. The logic works in such a way that debts will tend towards reduced relationships.

---

### **View debts**

The debt view shows all the debts the user has. Debts include both those that the user owes and those in which the user acts as a borrower.

---

### **View balance(may be)**

Balance is a function where the user can see the overall financial status. It works in such a way that it sums up all the debts that he borrowed and takes away from them all the debts that he must pay. In sum, this gives us the financial condition of the user, which allows us to build financial assumptions.

---

### **View history**

History gives the user access to all transactions. Transactions mean all loan records and repayment records made between users.

(add-ons)Сan be included a list of history in a certain period of time.

‌