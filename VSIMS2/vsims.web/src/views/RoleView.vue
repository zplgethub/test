<template>
	<a-page-header style="border: 1px solid rgb(235, 237, 240)" title="角色管理" sub-title="角色信息基本操作" />
	<a-form :model="formState" name="horizontal_query" layout="inline" autocomplete="off" @finish="onFinish" @finishFailed="onFinishFailed">
		<a-form-item label="角色名">
			<a-input v-model:value="formState.roleName"></a-input>
		</a-form-item>
		<a-form-item>
			<a-button type="primary" html-type="submit">查询</a-button>
		</a-form-item>
		<a-form-item>
			<a-button type="primary" @click="add">新增</a-button>
		</a-form-item>
	</a-form>
	<a-table :columns="columns" :data-source="dataSource" bordered :pagination="false" :row-key="record => record.id">
		<template #bodyCell="{ column, text, record }">
			<template v-if="[ 'name','teacher', 'createUser','lastEditUser'].includes(column.dataIndex)">
				<div>{{ text }}</div>
			</template>
			<template v-else-if="[ 'createTime','lastEditTime'].includes(column.dataIndex)">
				<div>{{formatDateString(text)}}</div>
			</template>
			<template v-else-if="column.dataIndex === 'operation'">
				<div class="editable-row-operations">
					<a @click="edit(record.id)">编辑</a>
					&nbsp;|&nbsp;
					<a @click="edit1(record.id)">分配菜单</a>
				</div>
			</template>
		</template>
	</a-table>
	<a-pagination v-model:current="pagination.current" :total="pagination.total" @change="change" />
	<a-modal ref="modalRoleRef" v-model:visible="visible" okText="保存" cancelText="取消" :wrap-style="{ overflow: 'hidden' }" @ok="handleOk">
		<div>
			<a-page-header style="border: 1px solid rgb(235, 237, 240)" title="角色管理" sub-title="新增或编辑角色信息" />
			<br />
			<a-form :model="addEditFormState">
				<a-form-item label="角色名">
					<a-input v-model:value="addEditFormState.name" />
				</a-form-item>
				<a-form-item label="描述">
					<a-input v-model:value="addEditFormState.description" />
				</a-form-item>
			</a-form>
		</div>
	</a-modal>
	<a-modal ref="modalMenuRef" v-model:visible="visible1" okText="保存" cancelText="取消" :wrap-style="{ overflow: 'hidden' }" @ok="handleOk1">
		<div class="rolemenu">
			<a-page-header style="border: 1px solid rgb(235, 237, 240)" title="角色分配菜单" sub-title="菜单分配信息" />
			<br />
			<div>
				<div style="margin-bottom: 8px">
					<span style="margin-left: 8px">
						<template v-if="hasSelected">
							{{ `选择了 ${selectedRowKeys.length} 个菜单` }}
						</template>
					</span>
				</div>
				<a-table :row-selection="{ selectedRowKeys: selectedRowKeys, onChange: onSelectChange }" :columns="columnMenus" :data-source="menuDataSource" :pagination="false" :row-key="record => record.id" />
			</div>
		</div>
	</a-modal>
</template>
<script lang="ts">
	import {
		computed,
		defineComponent,
		reactive,
		toRefs,
		ref,
		toRaw
	} from 'vue';
	import type {
		UnwrapRef
	} from 'vue';
	import {
		message
	} from 'ant-design-vue';
	import {
		getD,
		postD
	} from '../api/index.js';

	const columns = [{
			title: '角色名',
			dataIndex: 'name',
			key: 'name',
			align: 'center',
			width: '10%',
		},
		{
			title: '描述',
			dataIndex: 'description',
			key: 'description',
			align: 'center',
			width: '10%',
		},
		{
			title: '创建人员',
			dataIndex: 'createUser',
			key: 'createUser',
			align: 'center',
			width: '10%',
		},
		{
			title: '创建时间',
			dataIndex: 'createTime',
			key: 'createTime',
			align: 'center',
			width: '10%',
		},
		{
			title: '编辑人员',
			dataIndex: 'lastEditUser',
			key: 'lastEditUser',
			align: 'center',
			width: '10%',
		},
		{
			title: '编辑时间',
			dataIndex: 'lastEditTime',
			key: 'lastEditTime',
			align: 'center',
			width: '10%',
		},
		{
			title: '操作',
			dataIndex: 'operation',
			key: 'operation',
			align: 'center',
			width: '10%',
		},
	];
	type Key = string | number;
	const columnMenus = [{
			title: '菜单名称',
			dataIndex: 'name',
		},
		{
			title: '描述',
			dataIndex: 'description',
		},
	];
	
	interface DataItem {
		id: number,
		name: string,
		description: string,
		createTime: Date,
		createUser: string,
		lastEditTime: Date,
		lastEditUser: string
	}
	interface MenuDataItem {
		key: Key,
		id: number,
		name: string,
		description: string
	}
	interface FormState {
		roleName: string
	}
	const pagination = {
		total: 1,
		current: 1,
		pageSize: 10,
	};
	const dataSource: DataItem[] = [];
	const menuDataSource: MenuDataItem[] = [];
	export default defineComponent({
		setup() {
			const formatDateString = (value) => {
				var dt = new Date(value);
				let year = dt.getFullYear();
				let month = (dt.getMonth() + 1).toString().padStart(2, '0');
				let date = dt.getDate().toString().padStart(2, '0');
				let hour = dt.getHours().toString().padStart(2, '0');
				let minute = dt.getMinutes().toString().padStart(2, '0');
				let second = dt.getSeconds().toString().padStart(2, '0');
				return `${year}-${month}-${date} ${hour}:${minute}:${second}`;
			};
			const formState = reactive < FormState > ({
				roleName: '',
			});
			const addEditFormState = reactive({
				id: 0,
				name: '',
				description: '',
			});
			const visible = ref < boolean > (false);
			const visible1 = ref < boolean > (false);
			const add = () => {
				addEditFormState.id = -1;
				addEditFormState.name = '';
				addEditFormState.description = '';
				visible.value = true;
			};

			const handleOk = (e: MouseEvent) => {
				console.log(e);
				console.log(addEditFormState);
				var url = "";
				if (addEditFormState.id > 0) {
					url = "/api/Role/UpdateRole"; //编辑
				} else {
					url = "/api/Role/AddRole"; //新增
				}
				postD(url, {
					"id": addEditFormState.id > 0 ? addEditFormState.id : null,
					"name": addEditFormState.name,
					"description": addEditFormState.description,
					"createTime": new Date(),
					"createUser": 0,
					"lastEditTime": new Date(),
					"lastEditUser": 0
				}).then(res => {
					console.log(res);
					if (res.status == 200) {
						if (res.data == 0) {
							message.success('保存成功！');
							visible.value = false;
							var roleName = formState.roleName;
							getRoles(roleName);
						} else {
							message.error('保存失败！');
						}
					}
				});

			};
			const handleOk1 = () => {
				if (state.selectedRowKeys.length < 1) {
					message.error('请选择需要授权的角色');
					return;
				}
				var menuIds = state.selectedRowKeys.join(",");
				//debugger;
				var roleId = addEditFormState.id;
				let obj={"menuIds":menuIds,"roleId": roleId,};
				postD("/api/Role/SetRoleMenus",obj).then(res => {
					console.log(res);
					if (res.status == 200) {
						message.success('分配成功！');
						visible1.value = false;
					} else {
						message.error('分配失败！' + res.error);
					}
				});
			
			};
			const onFinish = (values: any) => {
				var roleName = formState.roleName;
				getRoles(roleName);
				console.log('Success:', values);
			};

			const onFinishFailed = (errorInfo: any) => {
				console.log('Failed:', errorInfo);
			};
			const selectedRowKeys :Key[]=[];
			const state = reactive({
				dataSource: dataSource,
				menuDataSource: menuDataSource,
				selectedRowKeys: selectedRowKeys
			});
			const hasSelected = computed(() => state.selectedRowKeys.length > 0);
			const onSelectChange = (selectedRowKeys: Key[]) => {
				console.log('selectedRowKeys changed: ', selectedRowKeys);
				state.selectedRowKeys = selectedRowKeys;
			};
			const getRoles = (roleName) => {
				dataSource.length = 0;
				getD('/api/Role/GetRoles', {
					"pageSize": pagination.pageSize,
					"pageNum": pagination.current,
					"roleName": roleName,
				}).then(res => {
					console.log(res);
					if (res.status == 200) {
						pagination.total = res.data.count; //总记录数
						console.log(pagination.total);
						for (let i = 0; i < res.data.items.length; i++) {
							dataSource.push({
								id: res.data.items[i].id,
								name: res.data.items[i].name,
								description: res.data.items[i].description,
								"createTime": res.data.items[i].createTime,
								"createUser": res.data.items[i].createUser,
								"lastEditTime": res.data.items[i].lastEditTime,
								"lastEditUser": res.data.items[i].lastEditUser,
							});
						}
						state.dataSource = [...dataSource];
					}
				});
			};
			getRoles(null);
			const getMenus = () => {
				menuDataSource.length = 0;
				getD('/api/Menu/GetMenus', {
					"pageSize": -1,
					"pageNum": 1,
					"menuName": '',
				}).then(res => {
					console.log(res);
					if (res.status == 200) {
						for (let i = 0; i < res.data.items.length; i++) {
							menuDataSource.push({
								id: res.data.items[i].id,
								key: res.data.items[i].id,
								name: res.data.items[i].name,
								description: res.data.items[i].description
							});
						}
						state.menuDataSource = [...menuDataSource];
					}
					console.log(menuDataSource);
				});
			}
			getMenus();
			const editableData: UnwrapRef < Record < string, DataItem >> = reactive({});

			const edit = (id: number) => {
				console.log(id);
				var obj = dataSource.filter(item => id === item.id)[0];
				addEditFormState.id = obj.id;
				addEditFormState.name = obj.name;
				addEditFormState.description = obj.description;
				visible.value = true;
				console.log(obj);
			};
			const edit1 = (id: number) => {
				console.log(id);
				var obj = dataSource.filter(item => id === item.id)[0];
				var roleId = obj.id;//当前用户的UserId
				selectedRowKeys.length=0;
				getD('/api/Role/GetRoleMenus',{
					roleId:roleId
				}).then(res=>{
					console.log(res);
					if(res.status==200){
						for(let i=0;i<res.data.count;i++){
							selectedRowKeys.push(res.data.items[i].menuId);
						}
						addEditFormState.id = obj.id;
						console.log(selectedRowKeys);
						state.selectedRowKeys=[...selectedRowKeys];
						visible1.value = true;
					}else{
						message.error('获取权限菜单失败');
					}
				});
				//console.log(obj);
			};
			const change = (pagination) => {
				var roleName = formState.roleName;
				getRoles(roleName);
				console.log(pagination);
			};
			const onSubmit = () => {
				console.log('submit!', toRaw(formState));
			};
			return {
				formatDateString,
				formState,
				addEditFormState,
				...toRefs(state),
				columns,
				columnMenus,
				editableData,
				add,
				edit,
				edit1,
				pagination,
				change,
				onFinish,
				onFinishFailed,
				onSubmit,
				visible,
				visible1,
				handleOk,
				handleOk1,
				hasSelected,
				onSelectChange,
			};
		},
	});
</script>
<style>
	.editable-row-operations a {
		margin-right: 8px;
	}

	.ant-form {
		height: 6vh;
		width: 100vh;
		background-color: transparent;
	}

	.ant-modal-content {
		height: 60vh !important;
	}

	.ant-modal-body .ant-input {
		width: 39vh;
	}

	.ant-modal-body {
		height: 55vh !important;
	}

	.ant-modal-body .ant-page-header {
		width: 46vh;
		padding: 0.5rem;
	}
	.ant-modal{
		height: 70vh !important;
	}
	.ant-form-item-label{
		min-width: 3.425rem;
	}
	.rolemenu .ant-table-tbody > tr > td{
		padding: 4px 4px;
	}
</style>