SELECT @solvSupportBrandId := Id FROM `Brand` WHERE `Name` = 'Solv Support' LIMIT 1;

Update `Brand`set InvoicingDashboardEnabled = 1 where Id = @solvSupportBrandId;