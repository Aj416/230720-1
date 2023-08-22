SELECT @amsId := Id FROM `brand` WHERE `ShortCode` = 'AMS' LIMIT 1;

update solv.tag set `Action` = 0, DiagnosisEnabled = 1
where BrandId = @amsId and `Name` = 'upsell-only';