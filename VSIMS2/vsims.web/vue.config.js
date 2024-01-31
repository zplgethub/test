// const {
// 	defineConfig
// } = require('@vue/cli-service');
const webpack = require('webpack');
module.exports = {
	css: {
		loaderOptions: {
			less: {
				lessOptions: {
					modifyVars: {
						'primary-color': '#1DA57A',
						'link-color': '#1DA57A',
						'border-radius-base': '2px',
					},
					javascriptEnabled: true,
				},
			},
		},
	},
	chainWebpack: config => {
		config
			.plugin('html')
			.tap(args => {
				args[0].title = 'SIMS'
				return args
			})
	},
	transpileDependencies: true,
	devServer: {
		open: true,
		host: 'localhost',
		port: 8080,
		https:false,
		//这里的ip和端口是前端项目的;下面为需要跨域访问后端项目
		proxy: {
			'/api': {
				target: 'http://localhost:5151/', //这里填入你要请求的接口的前缀
				ws: true, //代理websocked
				changeOrigin: true, //虚拟的站点需要更管origin
				secure: false, //是否https接口
			}
		}
	},
	//打开调试
	configureWebpack: {
		devtool: 'source-map'
	  }
}
