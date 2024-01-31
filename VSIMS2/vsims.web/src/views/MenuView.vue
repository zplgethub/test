<template>
	<a-page-header style="border: 1px solid rgb(235, 237, 240)" title="菜单管理" sub-title="菜单信息基本操作" />
	<a-form :model="formState" name="horizontal_query" layout="inline" autocomplete="off" @finish="onFinish" @finishFailed="onFinishFailed">
		<a-form-item label="菜单名">
			<a-input v-model:value="formState.menuName"></a-input>
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
			<template v-if="[ 'name','id','parentId','sortId','description','url'].includes(column.dataIndex)">
				<div>{{ text }}</div>
			</template>
			<template v-else-if="[ 'icon'].includes(column.dataIndex)">
				<div> 
					<a-image :width="20" :height="20" :src="text"/>
				</div>
			</template>
			<template v-else-if="column.dataIndex === 'operation'">
				<div class="editable-row-operations">
					<a @click="edit(record.id)">编辑</a>
				</div>
			</template>
		</template>
	</a-table>
	<a-pagination v-model:current="pagination.current" :total="pagination.total" @change="change" />
	<a-modal ref="modalRef" v-model:visible="visible" okText="保存" cancelText="取消" :wrap-style="{ overflow: 'hidden' }" @ok="handleOk">
		<div>
			<a-page-header style="border: 1px solid rgb(235, 237, 240)" title="菜单管理" sub-title="新增或编辑菜单信息" />
			<br />
			<a-form :model="addEditFormState">
				<a-form-item label="菜单名">
					<a-input v-model:value="addEditFormState.name" />
				</a-form-item>
				<a-form-item label="描述">
					<a-input v-model:value="addEditFormState.description" />
				</a-form-item>
				<a-form-item label="Url">
					<a-input v-model:value="addEditFormState.url" />
				</a-form-item>
				<a-form-item label="图标">
					<a-input v-model:value="addEditFormState.icon" />
				</a-form-item>
				<a-form-item label="父ID">
					<a-input v-model:value="addEditFormState.parentId" />
				</a-form-item>
				<a-form-item label="排序">
					<a-input v-model:value="addEditFormState.sortId" />
				</a-form-item>
			</a-form>
		</div>
	</a-modal>
</template>
<script lang="ts">
	import {
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

	const columns = [
		{
			title: '菜单ID',
			dataIndex: 'id',
			key: 'id',
			align: 'center',
			width: '10%',
		},
		{
			title: '菜单名称',
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
			title: 'Url',
			dataIndex: 'url',
			key: 'url',
			align: 'center',
			width: '10%',
		},
		{
			title: '图标',
			dataIndex: 'icon',
			key: 'icon',
			align: 'center',
			width: '10%',
		},
		{
			title: '父Id',
			dataIndex: 'parentId',
			key: 'parentId',
			align: 'center',
			width: '10%',
		},
		{
			title: '排序',
			dataIndex: 'sortId',
			key: 'sortId',
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
	interface DataItem {
		id: number,
		name: string,
		description: string,
		url:string,
		parentId:number,
		sortId:number,
		icon:string,
		createTime: Date,
		createUser: string,
		lastEditTime: Date,
		lastEditUser: string
	}
	interface FormState {
		menuName: string
	}
	const pagination = {
		total: 1,
		current: 1,
		pageSize: 10,
	};
	const dataSource: DataItem[] = [];
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
				menuName: '',
			});
			const addEditFormState = reactive({
				id: 0,
				name: '',
				description: '',
				url:'',
				parentId:0,
				sortId:0,
				icon:'',
			});
			const visible = ref < boolean > (false);
			const add = () => {
				addEditFormState.id = -1;
				addEditFormState.name = '';
				addEditFormState.description = '';
				addEditFormState.url = '';
				addEditFormState.parentId = 0;
				addEditFormState.sortId = 0;
				addEditFormState.icon='';
				visible.value = true;
			};

			const handleOk = (e: MouseEvent) => {
				console.log(e);
				console.log(addEditFormState);
				var url = "";
				if (addEditFormState.id > 0) {
					url = "/api/Menu/UpdateMenu"; //编辑
				} else {
					url = "/api/Menu/AddMenu"; //新增
				}
				postD(url, {
					"id": addEditFormState.id > 0 ? addEditFormState.id : null,
					"name": addEditFormState.name,
					"description": addEditFormState.description,
					"url":addEditFormState.url,
					"parentId":addEditFormState.parentId,
					"sortId":addEditFormState.sortId,
					"icon":'..'+addEditFormState.icon,
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
							var menuName = formState.menuName;
							getMenus(menuName);
						} else {
							message.error('保存失败！');
						}
					}
				});

			};
			const onFinish = (values: any) => {
				var menuName = formState.menuName;
				getMenus(menuName);
				console.log('Success:', values);
			};

			const onFinishFailed = (errorInfo: any) => {
				console.log('Failed:', errorInfo);
			};
			const state = reactive({
				dataSource: dataSource
			});

			const getMenus = (menuName) => {
				dataSource.length = 0;
				getD('/api/Menu/GetMenus', {
					"pageSize": pagination.pageSize,
					"pageNum": pagination.current,
					"menuName": menuName,
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
								url: res.data.items[i].url,
								parentId: res.data.items[i].parentId,
								sortId: res.data.items[i].sortId,
								icon: res.data.items[i].icon,
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
			getMenus(null);
			const editableData: UnwrapRef < Record < string, DataItem >> = reactive({});

			const edit = (id: number) => {
				console.log(id);
				var obj = dataSource.filter(item => id === item.id)[0];
				addEditFormState.id = obj.id;
				addEditFormState.name = obj.name;
				addEditFormState.description = obj.description;
				addEditFormState.url = obj.url;
				addEditFormState.parentId = obj.parentId;
				addEditFormState.sortId = obj.sortId;
				addEditFormState.icon=obj.icon;
				visible.value = true;
				console.log(obj);
			};

			const change = (pagination) => {
				var menuName = formState.menuName;
				getMenus(menuName);
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
				editableData,
				add,
				edit,
				pagination,
				change,
				onFinish,
				onFinishFailed,
				onSubmit,
				visible,
				handleOk,
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
		height: 50vh !important;
	}

	.ant-modal-body .ant-page-header {
		width: 46vh;
		padding: 0.5rem;
	}
	
	.ant-form-item-label{
		min-width: 3.425rem;
	}
</style>