<template>
	<div class="loginbody">
		<div id="login">
			<a-form :model="formState" name="basic" :label-col="{ span: 8 }" :wrapper-col="{ span: 16 }" autocomplete="off" @finish="onFinish" @finishFailed="onFinishFailed">
				<div id="title">
					<h1>人员信息管理系统</h1>
				</div>
				<a-form-item label="Username" name="username" :rules="[{ required: true, message: 'Please input your username!' }]">
					<a-input v-model:value="formState.username" />
				</a-form-item>

				<a-form-item label="Password" name="password" :rules="[{ required: true, message: 'Please input your password!' }]">
					<a-input-password v-model:value="formState.password" />
				</a-form-item>

				<a-form-item name="remember" :wrapper-col="{ offset: 8, span: 16 }">
					<a-checkbox v-model:checked="formState.remember">记住我</a-checkbox>
				</a-form-item>

				<a-form-item :wrapper-col="{ offset: 8, span: 16 }">
					<a-button style="margin-right: 10px;">Cancel</a-button>
					<a-button type="primary" html-type="submit">Submit</a-button>
				</a-form-item>
			</a-form>
		</div>
	</div>
</template>
<script lang="ts">
	import { message } from 'ant-design-vue';
	import {
		defineComponent,
		reactive,
	} from 'vue';
	import {
		useRouter,
		//useRoute
	} from 'vue-router';
	import { getD } from '../api/index.js';
	interface FormState {
		username: string;
		password: string;
		remember: boolean;
	}
	export default defineComponent({
		setup() {
			const router = useRouter();
			//const route= useRoute();
			const formState = reactive < FormState > ({
				username: '',
				password: '',
				remember: true,
			});
			const onFinish = (values: any) => {
				console.log(values);
				console.log('Success:', values);
				getD('/api/Login/Login',{"username":values.username,"password":values.password}).then(res=> {
					if(res.status==200){
						//返回成功
						if(res.data>0){
							sessionStorage['UserId']=values.username;
							sessionStorage['LoginId']=res.data;
							message.success('登录成功！');
							router.push('/home');
						}else{
							message.error('登录失败，用户名或密码错误！');
						}
					}else if(res.status==204){
						//没有返回
						message.error('用户名或密码错误！');
					}else{
						message.error('系统错误！');
					}
				})
				.catch(error=>{
					console.log(error)
				});
				//this.$router.push('/home');
			};

			const onFinishFailed = (errorInfo: any) => {
				console.log('Failed:', errorInfo);
			};
			return {
				formState,
				onFinish,
				onFinishFailed,
			};
		},
	});
</script>
<style scoped="scoped">
	.loginbody{
		background-image: url(../assets/bg.gif); 
		background-repeat: no-repeat;
		background-size: 100% 100%;
		width: 100%;
		height: 100vh;
	}
	#login{
		position: absolute;
		left: 35%;
		top: 30%;
	}
	#title h1{
		color: #FFFFFF;
	}
	.ant-form{
		width: 60vh;
		height: 35vh;
		background-color:rgba(10,10,10,0.2);
		padding: 0.625rem !important;
	}
	.ant-input{
		width: 80%;
	}
	.ant-input-password{
		width: 80%;
	}
</style>