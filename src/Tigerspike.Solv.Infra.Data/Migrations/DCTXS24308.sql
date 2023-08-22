UPDATE solv.Tag 
SET L1PostClosureDisable = 1
WHERE BrandId in ( SELECT id FROM solv.brand WHERE `Name` like '%HP%') and name like '%hardware%';