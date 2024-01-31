<template>
	<a-card hoverable style="width: 300px;">
		<template #cover>
			<img alt="example" src="https://gw.alipayobjects.com/zos/rmsportal/JiqGstEfoWAOHiTxclqi.png" />
		</template>
		<a-card-meta title="个人信息" description="个人信息展示">
			<template #avatar>
				<a-image :width="30" :height="30" src="/images/icon_student.png" />
			</template>
		</a-card-meta>
		<p>用户名: {{userName}}</p>
		<p>昵&nbsp;&nbsp;称: {{nickName}}</p>
		<p>角&nbsp;&nbsp;色: {{roleName}}</p>
	</a-card>
</template>
<script lang="ts">
	import {
		defineComponent,
		reactive,
		toRefs
	} from 'vue';
	import {
		useRouter
	} from 'vue-router';
	import {
		message
	} from 'ant-design-vue';
	import {
		getD
	} from '../api/index.js';
	
	export default defineComponent({
		components: {},
		setup() {
			const router = useRouter();
			const personal = reactive({
				userName: '',
				nickName: '',
				roleName: ''
			});
			const getPersonalInfo = () => {
				var userName = sessionStorage["UserId"];
				var userId = sessionStorage['LoginId'];
				if (userName == null || userId == null) {
					message.error('请先重新登录！');
					router.push('/login');
				}else{
					getD('/api/User/GetUser',{
						'id':userId
					}).then(res=>{
						if(res.status==200){
							personal.nickName=res.data.nickName;
						}
					});
					personal.userName=userName;
					personal.roleName=sessionStorage['roleName'];
				}
			}
			getPersonalInfo();
			return {
				...toRefs(personal),
			}
		}
	});
</script>
<style scoped>
	.ant-card {
		display: inline-block;
		vertical-align: top;
		margin: 10px;
	}
</style>
