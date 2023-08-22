SELECT @hpAmsId := Id FROM `brand` WHERE `Name` = 'HP Americas' LIMIT 1;

-- Update Brands tooltip data for HP Ams brand
update solv.brand set CategoryDescription = 'This tag tells us what the customer''s primary issue was. For example if you determine a customer''s issue is related to their battery, you should use the battery tag. It''s really important you use the correct tags - this helps us improve onboarding content.',
DiagnosisDescription = 'Select Yes → Correct if the level 1 had correctly diagnosed a hardware failure. If there was no hardware failure OR the level 1 did not perform troubleshooting, you should select No → Incorrect.',
SposDescription = 'Select Yes if you have resolved the initial query but customer is ALSO wanting to make a purchase. If customer only wishes to make a purchase you should use the up-sell only tag. You will be asked to complete a short description in a free text box e.g. customer wishes to extend their warranty.',
ValidTransferDescription = 'Select Yes if the issue was out of scope for a level 1 to resolve in full. Select No if the level 1 could have resolved the issue without a transfer. NOTE: Level 1s do not have the ability to check warranty status or determine what is in or out of warranty so your decision should be based on whether they could have resolved'
where Id = @hpAmsId;

-- Update Parent tag tooltip data for HP Ams brand
update solv.tag set Description = 'For use when you have performed all troubleshooting and determined that it is a hardware failure. NOTE: This will escalate the case to a level 2 to set up an onsite engineer visit.' 
where Name = 'hardware' and BrandId = @hpAmsId;
update solv.tag set Description = 'This tag should be used when you have successfully resolved the issue yourself.  After you select this tag you will be asked to pick a sub tag to which best describes the chat outcome.' 
where Name = 'software' and BrandId = @hpAmsId;
update solv.tag set Description = 'For use when the customer is ONLY wishing to make a purchase and required no other support. NOTE: this will transfer the chat directly to a sales agent! You will be asked to pick the sub tag which indicates what the customer is wishing to purchase AND complete a short description in a free text box e.g. customer wishes to extend their warranty.' 
where Name = 'upsell-only' and BrandId = @hpAmsId;

-- Update Software Sub tags tooltip data for HP Ams brand
update solv.tag set Description = 'customer went unresponsive during chat and is no longer responding. This tag should only be used AFTER you have followed the ''disconnected'' customer closing process from the script guide.' 
where Name = 'Customer disconnected' and BrandId = @hpAmsId;

update solv.tag set Description = 'you resolved a simple query which did not require troubleshooting e.g. how do I check my warranty online?' 
where Name = 'General query' and BrandId = @hpAmsId;

update solv.tag set Description = 'you have given the customer steps to try which cannot be performed immediately or where the customer may get disconnected.' 
where Name = 'Pending customer' and BrandId = @hpAmsId;

update solv.tag set Description = 'the issue is out of warranty and you have referred the customer to SmartFriend or web help.' 
where Name = 'Referred to website/self-help' and BrandId = @hpAmsId;

update solv.tag set Description = 'you performed troubleshooting steps and resolved the customer’s issue.' 
where Name = 'Troubleshooting fixed' and BrandId = @hpAmsId;

SELECT @hpIndId := Id FROM `brand` WHERE `Name` = 'HP India' LIMIT 1;

-- Update Brands tooltip data for HP Ams brand
update solv.brand set CategoryDescription = 'This tag tells us what the customer''s primary issue was. For example if you determine a customer''s issue is related to their battery, you should use the battery tag. It''s really important you use the correct tags - this helps us improve onboarding content.',
DiagnosisDescription = 'Select Yes if the level 1 had correctly diagnosed a hardware failure. If there was no hardware failure OR the level 1 did not perform troubleshooting, you should select No.',
SposDescription = 'Select Yes if you have resolved the initial query but customer is ALSO wanting to make a purchase. If customer only wishes to make a purchase you should use the up-sell only tag. You will be asked to complete a short description in a free text box e.g. customer wishes to extend their warranty.',
ValidTransferDescription = 'Select Yes if the issue was out of scope for a level 1 to resolve in full. Select No if the level 1 could have resolved the issue without a transfer. NOTE: Level 1s do not have the ability to check warranty status or determine what is in or out of warranty so your decision should be based on whether they could have resolved'
where Id = @hpIndId;

-- Update Parent tag tooltip data for HP Ams brand
update solv.tag set Description = 'For use when you have performed all troubleshooting and determined that it is a hardware failure. NOTE: This will escalate the case to a level 2 to set up an onsite engineer visit.' 
where Name = 'hardware' and BrandId = @hpIndId;
update solv.tag set Description = 'This tag should be used when you have successfully resolved the issue yourself.  After you select this tag you will be asked to pick a sub tag to which best describes the chat outcome.' 
where Name = 'software' and BrandId = @hpIndId;
update solv.tag set Description = 'For use when the customer is ONLY wishing to make a purchase and required no other support. NOTE: this will transfer the chat directly to a sales agent! You will be asked to pick the sub tag which indicates what the customer is wishing to purchase AND complete a short description in a free text box e.g. customer wishes to extend their warranty.' 
where Name = 'upsell-only' and BrandId = @hpIndId;

-- Update Software Sub tags tooltip data for HP Ams brand
update solv.tag set Description = 'customer went unresponsive during chat and is no longer responding. This tag should only be used AFTER you have followed the ''disconnected'' customer closing process from the script guide.' 
where Name = 'Customer disconnected' and BrandId = @hpIndId;

update solv.tag set Description = 'you resolved a simple query which did not require troubleshooting e.g. how do I check my warranty online?' 
where Name = 'General query' and BrandId = @hpIndId;

update solv.tag set Description = 'you have given the customer steps to try which cannot be performed immediately or where the customer may get disconnected.' 
where Name = 'Pending customer' and BrandId = @hpIndId;

update solv.tag set Description = 'the issue is out of warranty and you have referred the customer to SmartFriend or web help.' 
where Name = 'Referred to website/self-help' and BrandId = @hpIndId;

update solv.tag set Description = 'you performed troubleshooting steps and resolved the customer’s issue.' 
where Name = 'Troubleshooting fixed' and BrandId = @hpIndId;

update solv.tag set Description = 'the issue was out of warranty and you have referred the customer to the nearest SVC for support.' 
where Name = 'Referred to SVC' and BrandId = @hpIndId;