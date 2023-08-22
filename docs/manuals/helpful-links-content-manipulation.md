# Helpful links content upload

This document describes how to manipulate content of `Helpful links` page in the brand details on the Solver's console.

## Upload materials to S3

If the new/modified item requires resources to be hosted on our side (e.g. PDF files) you will have to first upload them into S3 bucket `solv-assets-bucket` on AWS.

Files should be uploaded into particular brands folder (create one if needed). 

There is no need to change any default configuration/fields of the uploaded file.

The uploaded file should be available at:
`https://assets.solvnow.com/<client>/<filename>` like for example:
`https://assets.solvnow.com/neato/Solv+Neato+chat+training.pdf`

## Insert new content (via SQL)

In order to insert new content for the brand you have to prepare SQL script which would do following steps:
* Select the desired brand id by it's name
* Either insert new section or select id of an existing one from the `InductionSection` table
* Insert new content item into `InductionSectionItem` supplying the desired SectionId and rest of the data

For technical reference, please follow the approach pursued in `integrations/neato/content/DCTXS2-774-helpful-links-new-content.sql`

## Insert new content (via API endpoint)

If you are to create totally new induction section from scratch and populate it with some links, then you can do it using an API endpoint instead of SQL.

In order to insert new content for the brand you have to prepare JSON structure that would contain:
* Name of the new section
* List of items & source links for them

For technical reference, please follow the approach pursued in `release-scripts/sprint-18/DCTXS2-1589-upload-training-material-for-hp-apac-solvers.http`

## Modify / remove existing content

If you wish to remove some of the existing content, the similar steps has to be done as for inserting. You will have to create a SQL script where you remove existing content rows (remember not to use direct GUIDs from environment, try to write migration code in a robust way, so it can be run on any environment) and remove no longer needed content from S3 bucket. 

Notice that there is also an option to just disable desired item temporarly if that's the case (set `Enabled` flag to `0` on desired `InductionSectionItem`).