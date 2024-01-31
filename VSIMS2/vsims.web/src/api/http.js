import axios from "axios"; // 引用axios

axios.defaults.baseURL =""; //'http://localhost:8080';

// const instance = axios.create({
// 	baseURL: config.baseUrl.dev,
// 	timeout: 60000,
// });
//get请求
export function get(url, params = {}) {
	return new Promise((resolve, reject) => {
		axios.get(url, {
				params: params,
			})
			.then((response) => {
				resolve(response);
			})
			.catch((err) => {
				reject(err);
			});
	});
}

//post请求
export function post(url, data = {}) {
	return new Promise((resolve, reject) => {
		axios.post(url, data).then(
			(response) => {
				resolve(response);
			},
			(err) => {
				reject(err);
			}
		);
	});
}