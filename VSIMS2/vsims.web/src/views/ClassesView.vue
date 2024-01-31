<template>
	<a-page-header style="border: 1px solid rgb(235, 237, 240)" title="班级管理" sub-title="班级信息基本操作" />
	<a-form :model="formState" name="horizontal_query" layout="inline" autocomplete="off" @finish="onFinish" @finishFailed="onFinishFailed">
		<a-form-item label="专业" name="no">
			<a-input v-model:value="formState.dept"></a-input>
		</a-form-item>
		<a-form-item label="年级" name="name">
			<a-input v-model:value="formState.grade"></a-input>
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
					<a @click="edit(record.id)">Edit</a>
				</div>
			</template>
		</template>
	</a-table>
	<a-pagination v-model:current="pagination.current" :total="pagination.total" @change="change" />
	<a-modal ref="modalRef" v-model:visible="visible" okText="保存" cancelText="取消" :wrap-style="{ overflow: 'hidden' }" @ok="handleOk">
		<div>
			<a-page-header style="border: 1px solid rgb(235, 237, 240)" title="班级管理" sub-title="新增或编辑班级" />
			<br />
			<a-form :model="addEditFormState">
				<a-form-item label="专业">
					<a-input v-model:value="addEditFormState.dept" />
				</a-form-item>
				<a-form-item label="年级">
					<a-input v-model:value="addEditFormState.grade" />
				</a-form-item>
				<a-form-item label="班级">
					<a-input v-model:value="addEditFormState.name" />
				</a-form-item>
				<a-form-item label="班主任">
					<a-input v-model:value="addEditFormState.headTeacher" />
				</a-form-item>
				<a-form-item label="班长">
					<a-select ref="select" v-model:value="addEditFormState.monitor" style="width: 200px">
						<a-select-option :value="item.id" v-for="(item) in dataMonitors" :key="item.id">{{item.name}}</a-select-option>
					</a-select>
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
	import { message } from 'ant-design-vue';
	import {
		getD,
		postD
	} from '../api/index.js';

	const columns = [
		{
			title: '专业',
			dataIndex: 'dept',
			key: 'dept',
			align: 'center',
			width: '10%',
		},
		{
			title: '年级',
			dataIndex: 'grade',
			key: 'grade',
			align: 'center',
			width: '10%',
		},
		{
			title: '班级',
			dataIndex: 'name',
			key: 'name',
			align: 'center',
			width: '10%',
		},
		{
			title: '班主任',
			dataIndex: 'headTeacher',
			key: 'headTeacher',
			align: 'center',
			width: '10%',
		},
		{
			title: '班长',
			dataIndex: 'monitorName',
			key: 'monitorName',
			align: 'center',
			width: '10%',
		},
		// {
		// 	title: '创建人员',
		// 	dataIndex: 'createUser',
		// 	key: 'createUser',
		// 	align: 'center',
		// 	width: '10%',
		// },
		// {
		// 	title: '创建时间',
		// 	dataIndex: 'createTime',
		// 	key: 'createTime',
		// 	align: 'center',
		// 	width: '10%',
		// },
		// {
		// 	title: '编辑人员',
		// 	dataIndex: 'lastEditUser',
		// 	key: 'lastEditUser',
		// 	align: 'center',
		// 	width: '10%',
		// },
		// {
		// 	title: '编辑时间',
		// 	dataIndex: 'lastEditTime',
		// 	key: 'lastEditTime',
		// 	align: 'center',
		// 	width: '10%',
		// },
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
		dept:string,
		grade:string,
		name: string,
		headTeacher:string,
		monitorName:string,
		monitor:number,
		createTime: Date,
		createUser: string,
		lastEditTime: Date,
		lastEditUser: string
	}
	interface FormState {
		dept: string;
		grade:string;
	}
	const pagination = {
		total: 1,
		current: 1,
		pageSize: 10,
	};
	interface MonitorItem {
		id: number,
		name: string
	}
	const dataSource: DataItem[] = [];
	const dataMonitors: MonitorItem[] = []; //班长列表
	export default defineComponent({
		setup() {
			const formatDateString=(value)=>{
				var dt = new Date(value);
				let year = dt.getFullYear();
				let month = (dt.getMonth() + 1).toString().padStart(2,'0');
				let date = dt.getDate().toString().padStart(2,'0');
				let hour = dt.getHours().toString().padStart(2,'0');
				let minute = dt.getMinutes().toString().padStart(2,'0');
				let second = dt.getSeconds().toString().padStart(2,'0');
				return `${year}-${month}-${date} ${hour}:${minute}:${second}`;
			};
			const formState = reactive < FormState > ({
				dept: '',
				grade:'',
			});
			const addEditFormState = reactive({
				id: 0,
				dept: '',
				grade:'',
				name:'',
				headTeacher:'',
				monitor:-1,
			});
			const getMonitors=()=>{
				getD('/api/Student/GetStudents', {
					"no": "",
					"name": "",
					"pageSize": 0,
					"pageNum": 0
				}).then(res => {
					console.log(res);
					if (res.status == 200) {
						for (let i = 0; i < res.data.items.length; i++) {
							dataMonitors.push({
								id: res.data.items[i].id,
								name: res.data.items[i].name,
							});
						}
						state.dataMonitors = [...dataMonitors];
					}
				});
			};
			getMonitors();
			const visible = ref < boolean > (false);
			const add = () => {
				addEditFormState.id = -1;
				addEditFormState.dept = '';
				addEditFormState.grade='';
				addEditFormState.name='';
				addEditFormState.headTeacher='';
				addEditFormState.monitor=0;
				visible.value = true;
			};

			const handleOk = (e: MouseEvent) => {
				console.log(e);
				console.log(addEditFormState);
				var url = "";
				if (addEditFormState.id >0) {
					url = "/api/Classes/UpdateClasses"; //编辑
				} else {
					url = "/api/Classes/AddClasses"; //新增
				}
				postD(url, {
					"id": addEditFormState.id>0?addEditFormState.id:null,
					"dept": addEditFormState.dept,
					"grade":addEditFormState.grade,
					"name":addEditFormState.name,
					"headTeacher":addEditFormState.headTeacher,
					"monitor":addEditFormState.monitor>0?addEditFormState.monitor:null,
					"createTime": new Date(),
					"createUser": 0,
					"lastEditTime": new Date(),
					"lastEditUser": 0
				}).then(res => {
					console.log(res);
					if(res.status==200){
						if(res.data==0){
							message.success('保存成功！');
							visible.value = false;
							var dept = formState.dept;
							var grade = formState.grade;
							getClassess(dept, grade);
						}else{
							message.error('保存失败！');
						}
					}
				});
				
			};
			const onFinish = (values: any) => {
				var dept = formState.dept;
				var grade = formState.grade;
				getClassess(dept, grade);
				console.log('Success:', values);
			};

			const onFinishFailed = (errorInfo: any) => {
				console.log('Failed:', errorInfo);
			};
			const state = reactive({
				dataSource: dataSource,
				dataMonitors:dataMonitors
			});
			
			const getClassess = (dept, grade) => {
				dataSource.length = 0;
				getD('/api/Classes/GetClassess', {
					"pageSize": pagination.pageSize,
					"pageNum": pagination.current,
					"dept": dept,
					"grade": grade
				}).then(res => {
					console.log(res);
					if (res.status == 200) {
						pagination.total = res.data.count; //总记录数
						console.log(pagination.total);
						for (let i = 0; i < res.data.items.length; i++) {
							dataSource.push({
								id: res.data.items[i].id,
								dept: res.data.items[i].dept,
								grade: res.data.items[i].grade,
								name: res.data.items[i].name,
								headTeacher: res.data.items[i].headTeacher,
								monitor:parseInt(res.data.items[i].monitor),
								monitorName:res.data.items[i].monitorName,
								"createTime":res.data.items[i].createTime,
								"createUser":res.data.items[i].createUser,
								"lastEditTime":res.data.items[i].lastEditTime,
								"lastEditUser":res.data.items[i].lastEditUser,
							});
						}
						state.dataSource = [...dataSource];
					}
				});
			};
			getClassess(null,null);
			const editableData: UnwrapRef < Record < string, DataItem >> = reactive({});

			const edit = (id: number) => {
				console.log(id);
				var classes = dataSource.filter(item => id === item.id)[0];
				addEditFormState.id = classes.id;
				addEditFormState.dept = classes.dept;
				addEditFormState.grade=classes.grade;
				addEditFormState.name = classes.name;
				addEditFormState.headTeacher=classes.headTeacher;
				addEditFormState.monitor=classes.monitor>0?classes.monitor:0;
				visible.value = true;
				console.log(classes);
			};
			
			const change = (pagination) => {
				var dept = formState.dept;
				var grade = formState.grade;
				getClassess(dept, grade);
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
				editingKey: '',
				editableData,
				edit,
				pagination,
				change,
				onFinish,
				onFinishFailed,
				onSubmit,
				visible,
				add,
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
		height: 50vh;
	}

	.ant-modal-body .ant-input {
		width: 40vh;
	}

	.ant-modal-body {
		height: 40vh;
	}

	.ant-modal-body .ant-page-header {
		width: 46vh;
		padding: 0.5rem;
	}
	.ant-form-item-label{
		min-width: 70px;
	}
</style>
