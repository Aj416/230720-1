cd ../../src/Tigerspike.Solv.Infra.Data
BRANCH=`git rev-parse --abbrev-ref HEAD`
JIRA=`grep -o "DCTXS2[[:punct:]][0-9]*" <<< $BRANCH`
SQL=`sed "s/-//g" <<<"$JIRA"`

# create migration for current branch
dotnet ef migrations add $JIRA

if [ "$1" = "-sql" ] ; then
	# create data migration file as well
	CS=`find Migrations -name "*_$JIRA.cs"`
    touch Migrations/$SQL.sql	
	PATTERN="}[protected"
	SWAP="    migrationBuilder.SqlFromFile(nameof($SQL)); \n        }\n\n        protected";
	perl -i -p0e "s/}.*?protected/$SWAP/s" $CS
fi

