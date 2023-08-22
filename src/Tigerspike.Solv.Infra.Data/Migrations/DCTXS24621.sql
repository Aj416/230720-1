SET @dellBillingDetailsId = UUID();

INSERT INTO `billingdetails` (`Id`, `Name`, `Email`, `VatNumber`, `CompanyNumber`, `Address`, `IsPlatformOwner`, `CreatedDate`)
VALUES
	(@dellBillingDetailsId, 'Dell Internationals Services India Pvt, LTD', null, null, null, 'Dell Internationals Services India Pvt, LTD \n Divashree Greens, 12/1, 12/1A, \n Koramangala Inner Ring Road, \n Domlur – Bangalore - 560079', 0, UTC_TIMESTAMP());

UPDATE `brand` SET BillingDetailsId = @dellBillingDetailsId WHERE Name = 'Dell';