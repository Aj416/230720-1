SELECT @bbbBrandId := Id FROM `Brand` WHERE `name` = 'BBB' LIMIT 1;
INSERT INTO `brandformfieldtype` (`Id`, `Name`) VALUES (3, 'select');
INSERT INTO `brandformfield` (`Id`, `Name`, `Title`, `TypeId`, `IsRequired`, `Validation`, `DefaultValue`, `Order`, `CreatedDate`, `ModifiedDate`, `BrandId`, `Options`)
VALUES
(UUID(), 'queryCategory', 'Query Category', 3, 1, 'required', '', 1, UTC_TIMESTAMP(), UTC_TIMESTAMP(), @bbbBrandId, 'option1,option2,option3'),
(UUID(), 'orderNumber', 'Order Number', 1, 1, 'required', '', 2, UTC_TIMESTAMP(), UTC_TIMESTAMP(), @bbbBrandId, NULL);