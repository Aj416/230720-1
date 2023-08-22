set @lastName = 'sample';
set @xspaceId = 'wb-rapunselc821792';
set @id = '';

set @xspaceSuffix = '@alibaba-inc.com';
select id, firstname, lastname, email, alternateemail from `user` where lastname = @lastName and (@id = '' OR id = @id);

UPDATE `user` set alternateemail = concat(@xspaceId, @xspaceSuffix) where lastname = @lastName and (@id = '' OR id = @id);
select id, firstname, lastname, email, alternateemail from `user` where lastname = @lastName and (@id = '' OR id = @id);