# Bulk insert advocate applications

## Step 1 - Acquire input from the business

The business should provide us with the CSV file with the data for the advocates to create. The file should have following format:

```
Email,FirstName,LastName
user01@mailinator.com,John,Doe
user02@mailinator.com,Jane,Doe
```

It can consist of any number of records. 

Business should also clarify:
- brands to which these users should be assigned to
- whether all of the advocates are internal agents or not

## Step 2 - Convert input to JSON format

You can use online [CSV to JSON converter](https://www.csvjson.com/csv2json) to convert CSV file into JSON format.
Copy the JSON output (array).

## Step 3 - Preapre API call

Prepare the API call to bulk create specified advocates.
Open `api-requests/admin.http` file in VS Code and search for a call to `bulk-insert`.
Prepare the message by:
- pasting the copied JSON applications into the `applications` field in the payload
- setting proper brand ids that would be assigned to those advocates,
- setting proper `source` that would be set for all the specified advocates

## Step 4 - Call the API

- select proper environment for the calls
- call the `getAdminToken` endpoint, to acquire authorization token
- call the previously prepared `bulk-insert` request
- you should be provided with the detailed results of what records where succesfully inserted and which of them failed and why