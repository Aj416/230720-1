Update solv.ticket set TagStatus = 0
where (Status = 4 and ClosedBy != 3) 
|| (Status = 3);

Update solv.ticket set TagStatus = 1
where (Status = 4 and ClosedBy = 3);