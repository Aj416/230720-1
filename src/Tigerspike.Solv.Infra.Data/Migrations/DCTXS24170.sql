SELECT @brandId := Id FROM `brand` WHERE `Name` = 'Samsung Italy' LIMIT 1;

update solv.brandadvocateresponseconfig set Content = 'This support ticket has been open for 2 day. We''ll close this now but if you need us again please submit a new query and our support team will be very pleased to assist you.' where `type` = 9 and brandid = @brandId;