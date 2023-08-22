# [MANUAL] Create new brand
Last updated: **10.04.2020**
***

This is a step-by-step manual for creating new brand in the Solv platform.
This manual assume that brand name we want to create is `BR1` (sometimes would also be referenced as `<brand_name>`).

## Step 0 - Gather business requirements

Required items:
- logo and it's thumbnail
- brand primary data
	- name
	- ticket price
	- fee percentage
	- VAT rate
	- [payment route](https://tigerspike.atlassian.net/wiki/spaces/MELCGE/pages/819234458/Payment+routes+definition) (if applicable)
	- induction "Done message" - message that goes below the hero section (and visible only when induction is completed) on Induction page
	- induction "Instructions" - message that goes inside the hero section on Induction page and Quiz page
	- invoicing dashboard (on/off)
- induction sections and items
- client accounts to be created (email, first name, last name)

If applicable:
- whitelist phrases
- brand invoicing details 
- agent accounts to be created (email, first name, last name)
- quiz data 
- escalation flow configuration
- web hooks (integration with third party systems, e.g. Zendesk)

## Step 1 - Upload external resources

Upload external resources to S3 bucket. The prerequistes include:
- logo (preferably PNG format)
- logo thumnbail (preferably PNG format)

Optional resources may include:
- helpful links section items

All resources should be placed in created `BR1` folder

For more details on how to do that please see: `docs/manuals/helpful-links-content-manipulation`


## Step 2 - Adjust brand setup file

Base on `create-brand.http` you can find in `integrations` folder create a new integration setup file for new brand.
This file should contain all neccessary steps for the new brand. Setup shall be then executed on all platform environments when required:
- dev
- SIT
- UAT
- Production
- Sandbox (if required)

## Artifacts

The following artifacts should be returned to businnes as the final results of this process:
- login credentials for client accounts (email / password)
- report from `createInternalAgents` API call (to check if all internal agent accounts were created succesfully)
- brand SDK api key (`@sdkApiKey`)