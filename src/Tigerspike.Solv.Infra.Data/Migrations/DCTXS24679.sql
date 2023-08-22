select @brandId := Id from solv.brand where ShortCode = 'WN';

update solv.brandformfield set `Options` = 'India,Egypt,Japan,Poland,Philippines,Ireland,Malaysia,Indonesia,Vietnam,USA,Brazil,China,Bulgaria,Poland/Italy,Australia,Canada,Brazil,Korea,Portugal,Canada/Portugal' 
where BrandId = @brandId and name = 'yourLocation';