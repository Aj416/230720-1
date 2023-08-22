-- set initial ordering on InductionSection elements
SET @order = 0;
UPDATE inductionsection SET `Order` = (@order:= @order + 1)
ORDER BY BrandId;

UPDATE
inductionsection i,
(
  SELECT MIN(`Order`) AS `Min`, BrandId FROM inductionsection GROUP BY BrandId
) rec
SET i.`Order` = i.`Order` - rec.`Min` + 1
WHERE i.BrandId = rec.BrandId;

-- set initial ordering on InductionSectionItem elements
SET @order = 0;
UPDATE inductionsectionitem SET `Order` = (@order:= @order + 1)
ORDER BY SectionId;

UPDATE
inductionsectionitem i,
(
  SELECT MIN(`Order`) AS `Min`, SectionId FROM inductionsectionitem GROUP BY SectionId
) rec
SET i.`Order` = i.`Order` - rec.`Min` + 1
WHERE i.SectionId = rec.SectionId;