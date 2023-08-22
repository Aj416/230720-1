SELECT @brandId := Id FROM `brand` WHERE `Name` = 'Dell' LIMIT 1;

UPDATE `solv`.`brand`
SET
`InductionInstructions` = 'Dell has put together this 101 guide to get you authorised to Solv for them. \n\n **NOTE: You must not copy, share or duplicate any of the contents of this section!** \n\n Dell is a global leader in home computing, you will be helping users of their products with out of warranty support, so it will be technical in nature on hardware, software and networking type questions. \n\n Support will be 24/7 but we think busiest from Monday to Friday, lighter over the weekend. We expect to see anything from 2000-3000 customers per month to start and grow. \n\n You will earn around $1.40 per chat ticket which will take between 3-10 mins of time to complete.'
WHERE `Id` = @brandId;