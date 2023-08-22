# noinspection SqlNoDataSourceInspectionForFile

Set @Date = UTC_DATE();
set @Phone1 = '987456';
set @Id1 = '22222222-2222-2222-2222-222222222222';
set @Id2 = '11111111-2222-3333-4444-555555555555';
set @Id3 = '22222222-3333-4444-5555-666666666666';
set @Id4 = '33333333-3333-4444-5555-666666666666';
set @Id5 = '44444444-3333-4444-5555-666666666666';
set @Id6 = '55555555-3333-4444-5555-666666666666';
set @Source = 'script';
-- Admin account
INSERT INTO `User`
	   (`Id`, `FirstName`, `LastName`, `Email`, `Enabled`, `Phone`, `CreatedDate`, `ModifiedDate`)
VALUES ('ad111111-1111-1111-1111-111111111111', 'Solv', 'Admin', 'solv.admin@mailinator.com', true, @Phone1, @Date, @Date);

-- Client account
INSERT INTO `User`
	   (`Id`, `FirstName`, `LastName`, `Email`, `Enabled`, `Phone`, `CreatedDate`, `ModifiedDate`)
VALUES ('c1222222-2222-2222-2222-222222222222', 'Solv', 'Client', 'solv.client@mailinator.com', true, @Phone1, @Date, @Date);

INSERT INTO `Client`
	   (Id, BrandId)
VALUES ('c1222222-2222-2222-2222-222222222222', @Id1);

-- Advocate 01
INSERT INTO `User`
	   (`Id`, `FirstName`, `LastName`, `Email`, `Enabled`, `Phone`, `CreatedDate`, `ModifiedDate`)
VALUES (@Id2, 'Solv', 'Advocate01', 'solv.adv01@mailinator.com', true, @Phone1, @Date, @Date);

INSERT INTO `AdvocateApplication`
	   (`Id`, `FirstName`, `LastName`, `Email`, `Phone`, `State`, `Country`, `ApplicationStatus` ,`Source`,`Token`,`CreatedDate`)
VALUES (@Id2, 'Solv', 'Advocate01', 'solv.adv01@mailinator.com',@Phone1,'DX','UAE',4,@Source,'11111222-2222-3333-4444-555555555111', @Date);

INSERT INTO `Advocate`
	   (`Id`, `PaymentMethodSetup`, `PaymentAccountId`, `Csat`, `ShowBrandNotification`, `VideoWatched`, `Practicing`, `PracticeComplete`)
VALUES (@Id2, true, '', 0.00, false, true, false, true);


INSERT INTO `AdvocateBrand`
		(`AdvocateId`, `BrandId`, `Authorized`, `ContractAccepted`, `Inducted`, `Enabled`, `CreatedDate`, `ModifiedDate`)
VALUES  (@Id2, @Id1, true, true, true, true, @Date, @Date);

-- Advocate 02
INSERT INTO `User`
	   (`Id`, `FirstName`, `LastName`, `Email`, `Enabled`, `Phone`, `CreatedDate`, `ModifiedDate`)
VALUES (@Id3, 'Solv', 'Advocate02', 'solv.adv02@mailinator.com', true, @Phone1, @Date, @Date);

INSERT INTO `AdvocateApplication`
	   (`Id`, `FirstName`, `LastName`, `Email`, `Phone`, `State`, `Country`, `ApplicationStatus` ,`Source`,`Token`,`CreatedDate`)
VALUES (@Id3, 'Solv', 'Advocate02', 'solv.adv02@mailinator.com',@Phone1,'DX','UAE',4,@Source,'22222111-3333-4444-5555-666666666222', @Date);

INSERT INTO `Advocate`
	   (`Id`, `PaymentMethodSetup`, `PaymentAccountId`, `Csat`, `ShowBrandNotification`, `VideoWatched`, `Practicing`, `PracticeComplete`)
VALUES (@Id3, true, '', 0.00, false, true, false, true);

INSERT INTO `AdvocateBrand`
		(`AdvocateId`, `BrandId`, `Authorized`, `ContractAccepted`, `Inducted`, `Enabled`, `CreatedDate`, `ModifiedDate`)
VALUES  (@Id3, @Id1, true, true, true, true, @Date, @Date);

-- Advocate 03
INSERT INTO `User`
	   (`Id`, `FirstName`, `LastName`, `Email`, `Enabled`, `Phone`, `CreatedDate`, `ModifiedDate`)
VALUES (@Id4, 'Solv', 'Advocate03', 'solv.adv03@mailinator.com', true, @Phone1, @Date, @Date);

INSERT INTO `AdvocateApplication`
	   (`Id`, `FirstName`, `LastName`, `Email`, `Phone`, `State`, `Country`, `ApplicationStatus` ,`Source`,`Token`,`CreatedDate`)
VALUES (@Id4, 'Solv', 'Advocate03', 'solv.adv03@mailinator.com',@Phone1,'DX','UAE',4,@Source,'33333111-3333-4444-5555-666666666111', @Date);

INSERT INTO `Advocate`
	   (`Id`, `PaymentMethodSetup`, `PaymentAccountId`, `Csat`, `ShowBrandNotification`, `VideoWatched`, `Practicing`, `PracticeComplete`)
VALUES (@Id4, true, '', 0.00, false, true, false, true);

INSERT INTO `AdvocateBrand`
		(`AdvocateId`, `BrandId`, `Authorized`, `ContractAccepted`, `Inducted`, `Enabled`, `CreatedDate`, `ModifiedDate`)
VALUES  (@Id4, @Id1, true, true, true, true, @Date, @Date);

-- SuperSolver 01
INSERT INTO `User`
	   (`Id`, `FirstName`, `LastName`, `Email`, `Enabled`, `Phone`, `CreatedDate`, `ModifiedDate`)
VALUES (@Id5, 'Solv', 'SuperSolver01', 'solv.supersolver01@mailinator.com', true, @Phone1, @Date, @Date);

INSERT INTO `Advocate`
	   (`Id`, `PaymentMethodSetup`, `PaymentAccountId`, `Csat`, `ShowBrandNotification`, `VideoWatched`, `Practicing`, `PracticeComplete`, `Super`)
VALUES (@Id5, true, '', 0.00, true, true, false, true, true);

INSERT INTO `AdvocateBrand`
		(`AdvocateId`, `BrandId`, `Authorized`, `ContractAccepted`, `Inducted`, `Enabled`, `CreatedDate`, `ModifiedDate`)
VALUES  (@Id5, @Id1, true, true, true, true, @Date, @Date);

-- SuperSolver 02
INSERT INTO `User`
	   (`Id`, `FirstName`, `LastName`, `Email`, `Enabled`, `Phone`, `CreatedDate`, `ModifiedDate`)
VALUES (@Id6, 'Solv', 'SuperSolver02', 'solv.supersolver02@mailinator.com', true, @Phone1, @Date, @Date);

INSERT INTO `Advocate`
	   (`Id`, `PaymentMethodSetup`, `PaymentAccountId`, `Csat`, `ShowBrandNotification`, `VideoWatched`, `Practicing`, `PracticeComplete`, `Super`)
VALUES (@Id6, true, '', 0.00, true, true, false, true, true);

INSERT INTO `AdvocateBrand`
		(`AdvocateId`, `BrandId`, `Authorized`, `ContractAccepted`, `Inducted`, `Enabled`, `CreatedDate`, `ModifiedDate`)
VALUES  (@Id6, @Id1, true, true, true, true, @Date, @Date);
