module.exports = {
    'env': {
        'node': true,
        'commonjs': true,
        'es6': true
    },
    'extends': 'eslint:recommended',
    'globals': {
        'Atomics': 'readonly',
        'SharedArrayBuffer': 'readonly'
    },
    'parserOptions': {
        'ecmaVersion': 2018,
        'sourceType': 'module'
    },
    'rules': {
        'indent': [
            'error',
            2
        ],
        'linebreak-style': [
            'error',
            'unix'
        ],
        'quotes': [
            'error',
            'single'
        ],
        'semi': [
            'error',
            'always'
        ],
        // Custom rule override below
        'space-before-function-paren': [
            'error',
            {
                'anonymous': 'always',
                'named': 'never',
                'asyncArrow': 'never'
            }
        ],
        'no-unused-vars': [
            'warn',
            {
                'vars': 'all',
                'args': 'after-used',
                'ignoreRestSiblings': false
            }
        ],
        'space-before-function-paren': 'off'
    }
};