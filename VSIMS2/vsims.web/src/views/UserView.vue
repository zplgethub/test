<template>
	<a-page-header style="border: 1px solid rgb(235, 237, 240)" title="用户管理" sub-title="用户信息基本操作" />
	<a-form :model="formState" name="horizontal_query" layout="inline" autocomplete="off" @finish="onFinish" @finishFailed="onFinishFailed">
		<a-form-item label="用户名">
			<a-input v-model:value="formState.userName"></a-input>
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
					<a @click="edit1(record.id)">授权</a>
				</div>
			</template>
		</template>
	</a-table>
	<a-pagination v-model:current="pagination.current" :total="pagination.total" @change="change" />
	<a-modal ref="modalUserRef" v-model:visible="visible" okText="保存" cancelText="取消" :wrap-style="{ overflow: 'hidden' }" @ok="handleOk">
		<div>
			<a-page-header style="border: 1px solid rgb(235, 237, 240)" title="用户管理" sub-title="新增或编辑用户信息" />
			<br />
			<a-form :model="addEditFormState">
				<a-form-item label="用户名">
					<a-input v-model:value="addEditFormState.userName" />
				</a-form-item>
				<a-form-item label="昵称">
					<a-input v-model:value="addEditFormState.nickName" />
				</a-form-item>
				<a-form-item label="密码">
					<a-input-password v-model:value="addEditFormState.password" />
				</a-form-item>
				<a-form-item label="确认密码">
					<a-input-password v-model:value="addEditFormState.confirmPassword" />
				</a-form-item>
			</a-form>
		</div>
	</a-modal>
	<a-modal ref="modalRoleRef" v-model:visible="visible1" okText="保存" cancelText="取消" :wrap-style="{ overflow: 'hidden' }" @ok="handleOk1">
		<div class="userrole">
			<a-page-header style="border: 1px solid rgb(235, 237, 240)" title="用户授权" sub-title="用户权限信息" />
			<br />
			<div>
				<div style="margin-bottom: 16px">
					<span style="margin-left: 8px">
						<template v-if="hasSelected">
							{{ `选择了 ${selectedRowKeys.length} 个角色` }}
						</template>
					</span>
				</div>
				<a-table :row-selection="{ selectedRowKeys: selectedRowKeys, onChange: onSelectChange }" :columns="columnRoles" :data-source="roleDataSource" :pagination="false" :row-key="record => record.id" />
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
			title: '用户名',
			dataIndex: 'userName',
			key: 'userName',
			align: 'center',
			width: '10%',
		},
		{
			title: '昵称',
			dataIndex: 'nickName',
			key: 'nickName',
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
	const columnRoles = [{
			title: '角色名称',
			dataIndex: 'name',
		},
		{
			title: '描述',
			dataIndex: 'description',
		},
	];

	interface DataItem {
		id: number,
			userName: string,
			nickName: string,
			createTime: Date,
			createUser: string,
			lastEditTime: Date,
			lastEditUser: string
	}
	interface RoleDataItem {
		key: Key,
			id: number,
			name: string,
			description: string
	}
	interface FormState {
		userName: string
	}
	const pagination = {
		total: 1,
		current: 1,
		pageSize: 10,
	};
	const dataSource: DataItem[] = [];
	const roleDataSource: RoleDataItem[] = [];
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
				userName: '',
			});
			const addEditFormState = reactive({
				id: 0,
				userName: '',
				nickName: '',
				password: '',
				confirmPassword: ''
			});
			const visible = ref < boolean > (false);
			const visible1 = ref < boolean > (false);
			const add = () => {
				addEditFormState.id = -1;
				addEditFormState.userName = '';
				addEditFormState.nickName = '';
				addEditFormState.password = '';
				addEditFormState.confirmPassword = '';
				visible.value = true;
			};

			const handleOk = (e: MouseEvent) => {
				console.log(e);
				console.log(addEditFormState);
				var url = "";
				if (addEditFormState.id > 0) {
					url = "/api/User/UpdateUser"; //编辑
				} else {
					url = "/api/User/AddUser"; //新增
				}
				postD(url, {
					"id": addEditFormState.id > 0 ? addEditFormState.id : null,
					"userName": addEditFormState.userName,
					"nickName": addEditFormState.nickName,
					"password": addEditFormState.password,
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
							var userName = formState.userName;
							getUsers(userName);
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
				var roleIds = state.selectedRowKeys.join(",");
				//debugger;
				var userId = addEditFormState.id;
				let obj={"roleIds":roleIds,"userId": userId,};
				postD("/api/User/SetUserRoles",obj).then(res => {
					console.log(res);
					if (res.status == 200) {
						message.success('授权成功！');
						visible1.value = false;
					} else {
						message.error('授权失败！' + res.error);
					}
				});

			};
			const onFinish = (values: any) => {
				var userName = formState.userName;
				getUsers(userName);
				console.log('Success:', values);
			};

			const onFinishFailed = (errorInfo: any) => {
				console.log('Failed:', errorInfo);
			};
			const selectedRowKeys: Key[] = [];
			const state = reactive({
				dataSource: dataSource,
				roleDataSource: roleDataSource,
				selectedRowKeys: selectedRowKeys
			});
			const hasSelected = computed(() => state.selectedRowKeys.length > 0);
			const onSelectChange = (selectedRowKeys: Key[]) => {
				console.log('selectedRowKeys changed: ', selectedRowKeys);
				state.selectedRowKeys = selectedRowKeys;
			};
			const getUsers = (userName) => {
				dataSource.length = 0;
				getD('/api/User/GetUsers', {
					"pageSize": pagination.pageSize,
					"pageNum": pagination.current,
					"userName": userName,
				}).then(res => {
					console.log(res);
					if (res.status == 200) {
						pagination.total = res.data.count; //总记录数
						console.log(pagination.total);
						for (let i = 0; i < res.data.items.length; i++) {
							dataSource.push({
								id: res.data.items[i].id,
								userName: res.data.items[i].userName,
								nickName: res.data.items[i].nickName,
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
			getUsers(null);
			const getRoles = () => {
				roleDataSource.length = 0;
				getD('/api/Role/GetRoles', {
					"pageSize": -1,
					"pageNum": 1,
					"roleName": '',
				}).then(res => {
					console.log(res);
					if (res.status == 200) {
						for (let i = 0; i < res.data.items.length; i++) {
							roleDataSource.push({
								id: res.data.items[i].id,
								key: res.data.items[i].id,
								name: res.data.items[i].name,
								description: res.data.items[i].description
							});
						}
						state.roleDataSource = [...roleDataSource];
					}
					console.log(roleDataSource);
				});
			}
			getRoles();
			const editableData: UnwrapRef < Record < string, DataItem >> = reactive({});

			const edit = (id: number) => {
				console.log(id);
				var obj = dataSource.filter(item => id === item.id)[0];
				addEditFormState.id = obj.id;
				addEditFormState.userName = obj.userName;
				addEditFormState.nickName = obj.nickName;
				visible.value = true;
				console.log(obj);
			};

			const edit1 = (id: number) => {
				console.log(id);
				var obj = dataSource.filter(item => id === item.id)[0];
				var userId = obj.id; //当前用户的UserId
				selectedRowKeys.length = 0;
				getD('/api/User/GetUserRoles', {
					userId: userId
				}).then(res => {
					console.log(res);
					if (res.status == 200) {
						for (let i = 0; i < res.data.count; i++) {
							selectedRowKeys.push(res.data.items[i].roleId);
						}
						addEditFormState.id = obj.id;
						console.log(selectedRowKeys);
						state.selectedRowKeys = [...selectedRowKeys];
						visible1.value = true;
					} else {
						message.error('获取用户权限失败');
					}
				});
				//console.log(obj);
			};

			const change = (pagination) => {
				var userName = formState.userName;
				getUsers(userName);
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
				columnRoles,
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
		height: 50vh;
	}

	.ant-modal-body .ant-input {
		width: 38vh;
	}

	.ant-modal-body .ant-input-password {
		width: 38vh;
	}

	.ant-modal-body {
		height: 40vh;
	}

	.ant-modal-body .ant-page-header {
		width: 46vh;
		padding: 0.5rem;
	}

	.ant-form-item-label {
		/* min-width: 4.375rem; */
		min-width: 70px;
	}

	.userrole .ant-table-tbody>tr>td {
		padding: 4px 4px;
	}
</style>
