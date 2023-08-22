SELECT @indId := Id FROM `brand` WHERE `ShortCode` = 'IND' LIMIT 1;
SELECT @amsId := Id FROM `brand` WHERE `ShortCode` = 'AMS' LIMIT 1;

update solv.tag set Description = 'you performed troubleshooting steps and resolved the customer’s issue.' 
where BrandId IN (@indId, @amsId) and Name = 'Troubleshooting fixed';