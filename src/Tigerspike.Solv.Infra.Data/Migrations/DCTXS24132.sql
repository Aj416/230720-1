SELECT @amsId := Id FROM `brand` WHERE `ShortCode` = 'AMS' LIMIT 1;
select @parentTagId := Id from solv.tag where `name` = 'upsell-only' and BrandId = @amsId;
update solv.tag set DiagnosisEnabled = null where ParentTagId = @parentTagId and BrandId = @amsId;