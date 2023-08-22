SET @hpAmsBillingDetailsId = UUID();

INSERT INTO `billingdetails` (`Id`, `Name`, `Email`, `VatNumber`, `CompanyNumber`, `Address`, `IsPlatformOwner`, `CreatedDate`)
VALUES
	(@hpAmsBillingDetailsId, 'HP Inc.', null, null, null, 'HP Inc. \n 1501 PAGE MILL RD. \n PALO ALTO, CA \n 94304-112', 0, UTC_TIMESTAMP());

UPDATE `brand` SET BillingDetailsId = @hpAmsBillingDetailsId WHERE ShortCode = 'AMS';
