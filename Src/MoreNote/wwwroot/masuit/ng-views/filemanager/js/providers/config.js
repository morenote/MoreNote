﻿(function(angular) {
    'use strict';
    angular.module('FileManagerApp').provider('fileManagerConfig', function() {

        var values = {
            appName: '懒得勤快的博客',
            defaultLang: 'zh_cn',

            listUrl: 'remotefile/handle',
            uploadUrl: 'remotefile/upload',
            renameUrl: 'remotefile/handle',
            copyUrl: 'remotefile/handle',
            moveUrl: 'remotefile/handle',
            removeUrl: 'remotefile/handle',
            editUrl: 'remotefile/handle',
            getContentUrl: 'remotefile/handle',
            createFolderUrl: 'remotefile/handle',
            downloadFileUrl: 'remotefile/handle',
            downloadMultipleUrl: 'remotefile/handle',
            compressUrl: 'remotefile/handle',
            extractUrl: 'remotefile/handle',
            permissionsUrl: 'remotefile/handle',
            basePath: '/',

            searchForm: true,
            sidebar: true,
            breadcrumb: true,
            allowedActions: {
                upload: true,
                rename: true,
                move: true,
                copy: true,
                edit: true,
                changePermissions: true,
                compress: true,
                compressChooseName: true,
                extract: true,
                download: true,
                downloadMultiple: true,
                preview: true,
                remove: true,
                createFolder: true,
                pickFiles: true,
                pickFolders: true
            },

            multipleDownloadFileName: 'files.zip',
            filterFileExtensions: [],
            showExtensionIcons: true,
            showSizeForDirectories: false,
            useBinarySizePrefixes: false,
            downloadFilesByAjax: true,
            previewImagesInModal: true,
            enablePermissionsRecursive: true,
            compressAsync: false,
            extractAsync: false,
            pickCallback: null,

            isEditableFilePattern: /\.(txt|diff?|patch|svg|asc|cnf|cfg|conf|html?|.\w{0,2}html|cfm|cgi|aspx?|ini|pl|py|md|css|cs|js|jsx|ts|tsx|jsp|log|htaccess|htpasswd|gitignore|gitattributes|env|json|atom|eml|rss|markdown|sql|\w{0,5}xml|xslt?|sh|rb|as|bat|cmd|cob|for|ftn|frm|frx|inc|lisp|scm|coffee|php[3-6]?|java|c|cbl|go|h|scala|vb|tmpl|lock|go|yml|yaml|tsv|lst|config|ashx|asax|\w{0,2}proj|map)$/i,
            isImageFilePattern: /\.(jpe?g|gif|bmp|png|svg|webp|ico|tiff?)$/i,
            isExtractableFilePattern: /\.(gz|tar|rar|g?zip|7z)$/i,
            tplPath: '/masuit/ng-views/filemanager/templates'
        };

        return {
            $get: function() {
                return values;
            },
            set: function (constants) {
                angular.extend(values, constants);
            }
        };

    });
})(angular);
