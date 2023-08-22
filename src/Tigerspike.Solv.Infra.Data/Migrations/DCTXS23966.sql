SELECT @indId := Id FROM `brand` WHERE `ShortCode` = 'IND' LIMIT 1;
SELECT @amsId := Id FROM `brand` WHERE `ShortCode` = 'AMS' LIMIT 1;

update solv.abandonreason set Description = 'This tag should be used when the customer is wishing to make a formal complaint; threatening to make complaints on social media or to an HP executive; or they are unhappy with the service you have provided or the service they have previously received and you are unable to deescalate'
where Name = 'Unhappy customer/complaint' and BrandId in (@indId,@amsId);

update solv.abandonreason set Description = 'This tag should be used when the customer is wishing to follow up on a previous case or issue they have raised and you are unable to see the previous chat history. An example of this may be where the customer has requested a repair or replacement and are awaiting contact from an on-site engineer.'
where Name = 'Customer has an existing query/case' and BrandId in (@indId,@amsId);

update solv.abandonreason set Description = 'This tag should be used when the customer has checked their warranty status and is reporting that the dates or type of cover are incorrect OR where you have advised them their issue is not covered under warranty and they are disputing this.'
where Name = 'Warranty dispute' and BrandId in (@indId,@amsId);

update solv.abandonreason set Description = 'For use you don’t know how to handle the query and there are no contents in the onboarding section of Solv. Note 95% of queries in this channel are in scope therefore you should use this option only where you genuinely cannot resolve. Escalates to 1 other crowd agent before reaching L2'
where Name = 'I don''t have the skills/knowledge to resolve' and BrandId in (@indId,@amsId);