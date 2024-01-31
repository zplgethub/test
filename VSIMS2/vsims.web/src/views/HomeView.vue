<template>
	<div id="layout">
		<a-layout>
			<a-layout-header>
				<div id="header">信息管理系统 SIMS</div>
				<div id="info">
					<span>Hello <a>Admin</a> | <a>Logout</a></span>
				</div>
			</a-layout-header>
			<a-layout>
				<a-layout-sider v-model:collapsed="collapsed">
					<a-button type="primary" style="margin-bottom: 5px" @click="toggleCollapsed">
						<MenuUnfoldOutlined v-if="collapsed" />
						<MenuFoldOutlined v-else />
					</a-button>
					<a-menu v-model:openKeys="openKeys" v-model:selectedKeys="selectedKeys" mode="inline" theme="dark" :inline-collapsed="collapsed">
						<a-menu-item key="1">
							<template #icon><HomeOutlined /></template>
							<router-link to="/Main">首 页</router-link>
						</a-menu-item>
						<a-sub-menu v-for="item,index in userRights.filter(item=>item.parentId==null)" :key="item.id">
							<template #icon v-if="index==1">
								<ReadOutlined />
							</template>
							<template #icon v-else>
								<AppstoreOutlined />
							</template>
							<template #title>
								<span class="onemenu">{{item.menuName}}</span>
							</template>
							<a-menu-item v-for="item1 in userRights.filter(item1=>item1.parentId==item.id)" :key="item1.id" >
								<a-image :width="20" :height="20" :src="item1.icon"/>
								<router-link :to="item1.url">{{item1.menuName}}</router-link>
							</a-menu-item>
						</a-sub-menu>
					</a-menu>
				</a-layout-sider>
				<a-layout-content>
					<router-view></router-view>
				</a-layout-content>
			</a-layout>
			<a-layout-footer>
				<div style="text-align: center;">&#169; 2023-2024 信息管理系统. All Rights Reserved | Design by zzz</div>
			</a-layout-footer>
		</a-layout>
	</div>
</template>

<script lang="ts">
	import { message } from 'ant-design-vue';
	import {
		useRouter,
		//useRoute
	} from 'vue-router';
	import {
		reactive,
		toRefs,
		watch
	} from 'vue';
	import {
		MenuFoldOutlined,
		MenuUnfoldOutlined,
		HomeOutlined,
		//PieChartOutlined,
		ReadOutlined,
		//DesktopOutlined,
		//InboxOutlined,
		AppstoreOutlined,
	} from '@ant-design/icons-vue';
	import {
		getD,
		postD
	} from '../api/index.js';
	interface userRightItem{
		id: number,
		roleName: string,
		menuName: string,
		icon: string,
		url: string,
		parentId: number|string,
		sortId: number|string
	}
	const userRights :userRightItem[]=[];
	export default {
		name: 'App',
		components: {
			MenuFoldOutlined,
			MenuUnfoldOutlined,
			HomeOutlined,
			//PieChartOutlined,
			ReadOutlined,
			//DesktopOutlined,
			//InboxOutlined,
			AppstoreOutlined,
		},
		setup() {
			const router = useRouter();
			const getUserRights=()=>{
				var userName = sessionStorage['UserId'];
				var userId = sessionStorage['LoginId'];
				if(userName==null || userId==null){
					message.error('请先重新登录！');
					router.push('/login');
				}else{
					userRights.length=0;
					getD('/api/Role/GetUserRights',{
						"userId":userId
					}).then(res=>{
						console.log(res);
						if(res.status==200){
							if(res.data.count>0){
								sessionStorage["roleName"]=res.data.items[0].roleName;
								for(let i=0;i<res.data.count;i++){
									userRights.push({
										"id": res.data.items[i].id,
										"roleName": res.data.items[i].roleName,
										"menuName": res.data.items[i].menuName,
										"icon": res.data.items[i].icon,
										"url": res.data.items[i].url,
										"parentId": res.data.items[i].parentId,
										"sortId": res.data.items[i].sortId
									});
								}
								
							}
							state.userRights=[...userRights];
							console.log(userRights);
						}
					});
				}
			};
			getUserRights();
			const state = reactive({
				collapsed: false,
				selectedKeys: ['1'],
				openKeys: ['sub1'],
				preOpenKeys: ['sub1'],
				userRights:userRights
			});

			watch(
				() => state.openKeys,
				(_val, oldVal) => {
					state.preOpenKeys = oldVal;
				},
			);
			const toggleCollapsed = () => {
				state.collapsed = !state.collapsed;
				state.openKeys = state.collapsed ? [] : state.preOpenKeys;
			};

			return {
				...toRefs(state),
				toggleCollapsed,
			};
		},
	}
</script>

<style scoped>
	#app {
		height: 100vh;
	}

	#layout {
		height: 100vh;
	}

	.ant-layout {
		height: inherit;
	}
	#header{
		font-size: xx-large;
		font-weight: bolder;
	}
	#info{
		position: absolute;
		right: 5px;
		top: 15px;
		height: 20px;
		display: inline;
	}
	.onemenu{
		font-weight: bold; 
		font-size: 16px;
	}
	.icon{
		width: 1.25rem;
		height: 1.25rem;
		vertical-align: text-top;
		margin-right: 3px;
	}
	#layout .code-box-demo {
		text-align: center;
	}

	#layout .ant-layout-header,
	#layout .ant-layout-footer {
		color: #fff;
		background: #7dbcea;
	}

	[data-theme='dark'] #layout .ant-layout-header {
		background: #6aa0c7;
	}

	[data-theme='dark'] #layout .ant-layout-footer {
		background: #6aa0c7;
	}

	#layout .ant-layout-footer {
		line-height: 1.5;
	}

	#layout .ant-layout-sider {
		color: #fff;
		line-height: 80px;
		background: #000000;
	}

	[data-theme='dark'] #layout .ant-layout-sider {
		background: #3499ec;
	}

	#layout .ant-layout-content {
		min-height: 120px;
		/* color: #fff; */
		/* line-height: 120px; */
		/* background: rgba(16, 142, 233, 1); */
	}

	[data-theme='dark'] #layout .ant-layout-content {
		background: #107bcb;
	}

	#layout>.code-box-demo>.ant-layout+.ant-layout {
		margin-top: 48px;
	}
</style>

