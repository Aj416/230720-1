SELECT @indId := Id FROM `Brand` WHERE `ShortCode` = 'IND' LIMIT 1;

update solv.brand set ProbingFormRedirectUrl = 'https://wa.me/912261014560', SkipCustomerForm = 1
where ID = @indId;

update solv.probingquestionoption set RedirectAnswer = 1 where Text = 'Yes' and Value is not null;