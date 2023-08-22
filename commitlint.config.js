// eslint-disable-next-line
module.exports = { 
    extends: ['@commitlint/config-conventional'],
    rules: { 
        'references-empty': [1, 'never'],
      },
    parserPreset: {
        parserOpts: {
            issuePrefixes: ['DCTXS2-']
        }
    },
};
