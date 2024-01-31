<template>
	<a-page-header style="border: 1px solid rgb(235, 237, 240)" title="学生管理" sub-title="学生信息基本操作" />
	<a-form :model="formState" name="horizontal_query" layout="inline" autocomplete="off" @finish="onFinish" @finishFailed="onFinishFailed">
		<a-form-item label="学号" name="no">
			<a-input v-model:value="formState.no"></a-input>
		</a-form-item>
		<a-form-item label="姓名" name="name">
			<a-input v-model:value="formState.name"></a-input>
		</a-form-item>
		<a-form-item>
			<a-button type="primary" html-type="submit">查询</a-button>
		</a-form-item>
		<a-form-item>
			<a-button type="primary" @click="addStudent">新增</a-button>
		</a-form-item>
	</a-form>
	<a-table :columns="columns" :data-source="dataSource" bordered :pagination="false" :row-key="record => record.id">
		<template #bodyCell="{ column, text, record }">
			<template v-if="[ 'no','name', 'sge', 'dex','classesName'].includes(column.dataIndex)">
				<div>{{ text }}</div>
			</template>
			<template v-else-if="column.dataIndex === 'operation'">
				<div class="editable-row-operations">
					<a @click="edit(record.key)">Edit</a>
				</div>
			</template>
		</template>
	</a-table>
	<a-pagination v-model:current="pagination.current" :total="pagination.total" @change="change" />
	<a-modal v-model:visible="visible" okText="保存" cancelText="取消" :wrap-style="{ overflow: 'hidden' }" @ok="handleOk">
		<div>
			<a-page-header style="border: 1px solid rgb(235, 237, 240)" title="学生管理" sub-title="新增或编辑学生" />
			<br />
			<a-form :model="addEditFormState">
				<a-form-item label="学号">
					<a-input v-model:value="addEditFormState.no" />
				</a-form-item>
				<a-form-item label="姓名">
					<a-input v-model:value="addEditFormState.name" />
				</a-form-item>
				<a-form-item label="年龄">
					<a-input v-model:value="addEditFormState.age" />
				</a-form-item>
				<a-form-item label="性别">
					<a-radio-group v-model:value="addEditFormState.sex">
						<a-radio :value="true">男</a-radio>
						<a-radio :value="false">女</a-radio>
					</a-radio-group>
				</a-form-item>
				<a-form-item label="班级">
					<a-select ref="select" v-model:value="addEditFormState.classes" style="width: 200px">
						<a-select-option :value="item.id" v-for="(item) in dataClasses" :key="item.id">{{item.name}}</a-select-option>
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
			title: '学号',
			dataIndex: 'no',
			key: 'no',
			align: 'center',
			width: '20%',
		},
		{
			title: '姓名',
			dataIndex: 'name',
			key: 'name',
			align: 'center',
			width: '20%',
		},
		{
			title: '年龄',
			dataIndex: 'age',
			key: 'age',
			align: 'center',
			width: '15%',
		},
		{
			title: '性别',
			dataIndex: 'sex',
			key: 'sex',
			align: 'center',
			width: '10%',
		},
		{
			title: '班级',
			dataIndex: 'classesName',
			key: 'classesName',
			align: 'center',
			width: '20%',
		},
		{
			title: '操作',
			dataIndex: 'operation',
			key: 'operation',
			align: 'center',
		},
	];
	interface DataItem {
		id: number,
		key: string,
		no: string,
		name: string,
		age: number,
		sex: string,
		sexValue: boolean,
		classesId: string,
		classesName: string
	}
	interface FormState {
		no: string;
		name: string;
	}
	interface ClassesItem {
		id: number,
		name: string
	}
	const pagination = {
		total: 1,
		current: 1,
		pageSize: 10,
	};
	const dataClasses: ClassesItem[] = []; //班级列表
	const dataSource: DataItem[] = [];
	export default defineComponent({
		setup() {
			const modalRef = ref(null);
			const formState = reactive < FormState > ({
				no: '',
				name: '',
			});
			const addEditFormState = reactive({
				id: 0,
				no: '',
				name: '',
				age: 0,
				sex: false,
				classes: '',
			});
			const visible = ref < boolean > (false);
			const addStudent = () => {
				addEditFormState.id = -1;
				addEditFormState.no = '';
				addEditFormState.name = '';
				addEditFormState.age = 0;
				addEditFormState.classes = '';
				visible.value = true;
			};

			const handleOk = (e: MouseEvent) => {
				console.log(e);
				console.log(addEditFormState);
				var url = "";
				if (addEditFormState.id >0) {
					url = "/api/Student/UpdateStudent"; //编辑
				} else {
					url = "/api/Student/AddStudent"; //新增
				}
				postD(url, {
					"id": addEditFormState.id>0?addEditFormState.id:null,
					"no": addEditFormState.no,
					"name": addEditFormState.name,
					"age": addEditFormState.age,
					"sex": addEditFormState.sex,
					"classesId": addEditFormState.classes,
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
							var no = formState.no;
							var name = formState.name;
							getStudents(no, name);
						}else{
							message.error('保存失败！');
						}
					}
				});
				
			};
			const onFinish = (values: any) => {
				var no = values.no;
				var name = values.name;
				getStudents(no, name);
				console.log('Success:', values);
			};

			const onFinishFailed = (errorInfo: any) => {
				console.log('Failed:', errorInfo);
			};
			const state = reactive({
				dataSource: dataSource,
				dataClasses: dataClasses
			});
			const getClasses=()=>{
				getD('/api/Classes/GetClassess', {
					"dept": "",
					"grade": "",
					"pageSize": 0,
					"pageNum": 0
				}).then(res => {
					console.log(res);
					if (res.status == 200) {
						for (let i = 0; i < res.data.items.length; i++) {
							dataClasses.push({
								id: res.data.items[i].id,
								name: res.data.items[i].dept +res.data.items[i].grade+ res.data.items[i].name,
							});
						}
						state.dataClasses = [...dataClasses];
					}
				});
			};
			getClasses();
			const getStudents = (no, name) => {
				dataSource.length = 0;
				getD('/api/Student/GetStudents', {
					"pageSize": pagination.pageSize,
					"pageNum": pagination.current,
					"no": no,
					"name": name
				}).then(res => {
					console.log(res);
					if (res.status == 200) {
						pagination.total = res.data.count; //总记录数
						console.log(pagination.total);
						for (let i = 0; i < res.data.items.length; i++) {
							dataSource.push({
								id: res.data.items[i].id,
								key: res.data.items[i].id.toString(),
								no: res.data.items[i].no,
								name: res.data.items[i].name,
								age: res.data.items[i].age,
								sex: res.data.items[i].sex ? "男" : "女",
								sexValue: res.data.items[i].sex,
								classesId: res.data.items[i].classesId,
								classesName: res.data.items[i].classesName,
							});
						}
						state.dataSource = [...dataSource];
					}
				});
			};
			getStudents(null,null);
			const editableData: UnwrapRef < Record < string, DataItem >> = reactive({});

			const edit = (key: string) => {
				console.log(key);
				var student = dataSource.filter(item => key === item.key)[0];
				addEditFormState.id = student.id;
				addEditFormState.no = student.no;
				addEditFormState.name = student.name;
				addEditFormState.age = student.age;
				addEditFormState.sex = student.sexValue;
				addEditFormState.classes = student.classesId;
				visible.value = true;
				console.log(student);
			};
			
			const change = (pagination) => {
				var no = formState.no;
				var name = formState.name;
				getStudents(no, name);
				console.log(pagination);
			};
			const onSubmit = () => {
				console.log('submit!', toRaw(formState));
			};
			return {
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
				addStudent,
				handleOk,
				modalRef
				//modalTitleRef,
				//transformStyle,
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
	
	.ant-modal{
		height: 40vh;
	}
	.ant-modal-content {
		height: 50vh !important;
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
</style>
