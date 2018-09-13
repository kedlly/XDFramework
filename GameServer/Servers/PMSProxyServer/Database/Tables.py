#coding:utf8

from sqlalchemy import Column, Integer, String, create_engine, Date, Boolean
from sqlalchemy.orm import sessionmaker
from sqlalchemy.ext.declarative import declarative_base

from Configure import DATA_SOURCE_NAME

Base = declarative_base()

class WorkTicket_Class_One(Base):
	
	__tablename__ = 'BUSS_WORK_TICKET_1'
	
	WORK_WRITE_ID				=	Column('WORK_WRITE_ID',				String(50), primary_key=True)		# 厂站第一种工作票ID
	WORK_WRITE_NO				=	Column('WORK_WRITE_NO',				String(50)) 						# 工作票票号
	WORKBILL					=	Column('WORKBILL',					String(50))  						# 工单
	WHETHER_EXTERIOR			=	Column('WHETHER_EXTERIOR', 			String(50))							# 是否外来单位
	TDSQD						=	Column('TDSQD',						String(50))							# 停电申请单
	DH_WRITE_NO					=	Column('DH_WRITE_NO',				String(50))							# 动火工作票票号
	DH_WRITE_NAME				=	Column('DH_WRITE_NAME',				String(50))							# 动火工作票名称
	WORK_PRINCIPAL_NAME			=	Column('WORK_PRINCIPAL_NAME',		String(50))							# 工作负责人 | 工作负责人名称
	PLAN_BEGIN_DATE				=	Column('PLAN_BEGIN_DATE',			Date()	)							# 计划开始时间
	DEPARTMENT_ID				=	Column('DEPARTMENT_ID',				String(50))							# 单位和班组ID
	DEPARTMENT_NAME				=	Column('DEPARTMENT_NAME',			String(50))							# 单位和班组
	#PLAN_END_TIME				=	Column('PLAN_END_TIME',				Date	)							# 计划结束时间
	CZ_TYPE						=	Column('CZ_TYPE',					String(50))							# 厂站类型
	CZ_TYPE_NO					=	Column('CZ_TYPE_NO',				String(50))							# 厂站类型编号
	CZ_ID 						=	Column('CZ_ID',						String(50))							# 厂站ID
	CZ_NAME						=	Column('CZ_NAME',					String(50))							# 厂站名称
	YWDW_ID						=	Column('YWDW_ID',					String(50))							# 运行单位ID
	YWDW_NAME					=	Column('YWDW_NAME',					String(50))							# 运行单位名称
	WORK_PRINCIPAL_ID			=	Column('WORK_PRINCIPAL_ID',			String(50))							# 工作负责人ID
	#WORK_PRINCIPAL_NAME			=	Column('WORK_PRINCIPAL_NAME',		String(50))							# 工作负责人名称
	WORK_MOBILE_PHONE			=	Column('WORK_MOBILE_PHONE',			String(50))							# 工作负责人电话
	#PLAN_START_TIME				=	Column('PLAN_START_TIME',			Date)								# 计划开始时间
	PLAN_END_TIME				=	Column('PLAN_END_TIME',				Date())								# 计划结束时间
	WORK_PERSONNEL				=	Column('WORK_PERSONNEL',			String(50))							# 工作班人员
	WORK_PERSONNEL_COUNT		=	Column('WORK_PERSONNEL_COUNT',		String(50))							# 工作班人员数量
	WORK_TASK					=	Column('WORK_TASK',					String(50))							# 工作任务
	WORK_PLACE					=	Column('WORK_PLACE',				String(50))							# 工作地点
	ZDZZJHR_ID					=	Column('ZDZZJHR_ID',				String(50))							# 指定专责监护人ID
	ZDZZJHR_NAME				=	Column('ZDZZJHR_NAME',				String(50))							# 指定专责监护人姓名
	YLDLQ						=	Column('YLDLQ',						String(50))							# 应拉断路器
	YLGLKG						=	Column('YLGLKG',					String(50))							# 应拉隔离开关（刀闸）
	YTQXGZLDY_DY_ECHL			=	Column('YTQXGZLDY_DY_ECHL',			String(50))							# 应投切相关直流电源、低压及二次回路
	YHJDDZ_ZSJDX_YSJYDB			=	Column('YHJDDZ_ZSJDX_YSJYDB',		String(50))							# 应合接地刀闸、装设接地线、应设绝缘挡板
	YSZD_YGBSP					=	Column('YSZD_YGBSP',				String(50))							# 应设遮栏、应挂标示牌
	QTAQCS_ZYSX					=	Column('QTAQCS_ZYSX',				String(50))							# 其他安全措施和注意事项
	IS_XLDCJD					=	Column('IS_XLDCJD',					String(50))							# 是否要求线路对侧接地
	IS_COMMUNICATION			=	Column('IS_COMMUNICATION',			String(50))							# 是否通信票
	ECCSD_NUM					=	Column('ECCSD_NUM',					String(50))							# 二次措施单数量
	UNDERWRITE					=	Column('UNDERWRITE',				String(50))							# 工作票签发人
	SIGN_TIME					=	Column('SIGN_TIME',					String(50))							# 签发时间
	UNDERWRITE_NAME				=	Column('UNDERWRITE_NAME',			String(50))							# 工作票签发人名称
	UNDERWRITE_SIGNATURE		=	Column('UNDERWRITE_SIGNATURE',		String(50))							# 工作票会签人 *
	CHECK_TIME					=	Column('CHECK_TIME',				String(50))							# 会签时间
	UNDERWRITE_SIGNATURE_NAME	=	Column('UNDERWRITE_SIGNATURE_NAME',	String(50))							# 工作票会签人名称 *
	WORK_PRIME_ID				=	Column('WORK_PRIME_ID',				String(50))							# 值班负责人ID
	WORK_PRIME_NAME				=	Column('WORK_PRIME_NAME',			String(50))							# 值班负责人姓名


class WorkTicket_Class_Two(Base):
	
	__tablename__ = 'BUSS_WORK_TICKET_2'
	
	
	WORK_WRITE_ID				=	Column('WORK_WRITE_ID',				String(50), primary_key=True)		 # 厂站第二种工作票ID
	WORK_WRITE_NO				=	Column('WORK_WRITE_NO',				String(50))							 # 工作票票号
	WHETHER_EXTERIOR			=	Column('WHETHER_EXTERIOR',			String(50))							 # 是否外来单位
	WORKBILL					=	Column('WORKBILL',					String(50))							 # 工单
	DH_WRITE_NO					=	Column('DH_WRITE_NO',				String(50))							 # 动火工作票票号
	DH_WRITE_NAME				=	Column('DH_WRITE_NAME',				String(50))							 # 动火工作票名称
	IS_COMMUNICATION_TICKET		=	Column('IS_COMMUNICATION_TICKET',	Boolean())							 # 是否通信票
	DEPARTMENT_NAME				=	Column('DEPARTMENT_NAME',			String(50))							 # 单位和班组
	SUBSTATION_TYPE				=	Column('SUBSTATION_TYPE',			String(50))							 # 厂站类型
	SUBSTATION_ID				=	Column('SUBSTATION_ID',				String(50))							 # 厂站ID
	SUBSTATION_NAME				=	Column('SUBSTATION_NAME',			String(50))							 # 厂站
	OPERATION_UNIT				=	Column('OPERATION_UNIT',			String(50))							 # 运维单位
	WORK_MASTER_UID				=	Column('WORK_MASTER_UID',			String(50))							 # 工作负责人ID
	WORK_MASTER_UNAME			=	Column('WORK_MASTER_UNAME',			String(50))							 # 工作负责人姓名
	WORK_MASTER_UPHONE			=	Column('WORK_MASTER_UPHONE',		String(50))							 # 负责人电话
	PLAN_BEGIN_DATE				=	Column('PLAN_BEGIN_DATE',			Date()	)							 # 计划开始时间
	PLAN_END_TIME				=	Column('PLAN_END_TIME',				Date()	)							 # 计划结束时间
	WORK_MEMBER					=	Column('WORK_MEMBER',				String(50))							 # 工作班人员
	WORK_TASK					=	Column('WORK_TASK',					String(250))						 # 工作任务
	WORK_PLACE					=	Column('WORK_PLACE',				String(250))						 # 工作地点
	HIGHP_DEVICE_STATE			=	Column('HIGHP_DEVICE_STATE',		String(50))							 # 相关高压设备状态
	POWER_CIRCLE_STATE			=	Column('POWER_CIRCLE_STATE',		String(50))							 # 相关直流，低压及二次回路状态
	DCPOWER_LOWP_CIRCLE			=	Column('DCPOWER_LOWP_CIRCLE',		String(50))							 # 应投切相关直流电源、低压及二次回路
	BILLBOARD					=	Column('BILLBOARD',					String(50))							 # 应设遮栏、应挂标示牌
	OTHER_CARE					=	Column('OTHER_CARE',				String(50))							 # 其他安全措施和注意事项
	IS_SECONDBILL				=	Column('IS_SECONDBILL',				String(50))							 # 是否办理二次设备及回路工作安全技术措施单
	SECONDBILL_COUNT			=	Column('SECONDBILL_COUNT',			String(50))							 # 二次措施单数量
	TICKET_SIGN_UID				=	Column('TICKET_SIGN_UID',			String(50))							 # 工作票签发人ID
	TICKET_SIGN_UNAME			=	Column('TICKET_SIGN_UNAME',			String(50))							 # 工作票签发人
	TICKET_CREAT_TIME			=	Column('TICKET_CREAT_TIME',			Date()	)							 # 签发时间
	WATCH_UID					=	Column('WATCH_UID',					String(50))							 # 值班负责人ID
	WATCH_UNAME					=	Column('WATCH_UNAME',				String(50))							 # 值班负责人
	RECEIVE_TIME				=	Column('RECEIVE_TIME',				Date()	)							 # 收到工作票的时间
	WHETHER_MEET_SAFTY			=	Column('WHETHER_MEET_SAFTY',		String(50))			#Boolean()		 # 是否满足工作要求的安全措施
	SUPPLEMENT_SAFTY			=	Column('SUPPLEMENT_SAFTY',			String(50))							 # 需补充或调整的安全措施
	ELE_GENERATRIX_WIRE			=	Column('ELE_GENERATRIX_WIRE',		String(50))							 # 带电的母线、导线
	ELE_SWITCH					=	Column('ELE_SWITCH',				String(50))							 # 带电的隔离开关
	ELE_PART					=	Column('ELE_PART',					String(50))							 # 其他保留的带电部位
	PERMISSION_OTHER_CARE		=	Column('PERMISSION_OTHER_CARE',		String(50))							 # 其他安全注意事项
	PERMISSION_TIME				=	Column('PERMISSION_TIME',			Date()	)							 # 现场满足工作要求时间
	WORK_PERMISSION_UID			=	Column('WORK_PERMISSION_UID',		String(50))							 # 工作许可人ID
	WORK_PERMISSION_UNME		=	Column('WORK_PERMISSION_UNME',		String(50))							 # 工作许可人
	WORK_END_TIME				=	Column('WORK_END_TIME',				Date())								 # 工作结束时间
	CHANGE_SIGN_UID				=	Column('CHANGE_SIGN_UID',			String(50))							 # 工作票签发人ID
	CHANGE_SIGN_UNAME			=	Column('CHANGE_SIGN_UNAME',			String(50))							 # 工作票签发人
	CHANGE_PRINCIPAL_UID		=	Column('CHANGE_PRINCIPAL_UID',		String(50))							 # 原工作负责人ID
	CHANGE_PRINCIPAL_UNAME		=	Column('CHANGE_PRINCIPAL_UNAME',	String(50))							 # 原工作负责人
	CHANGE_NEW_PRINCIPAL_UID	=	Column('CHANGE_NEW_PRINCIPAL_UID',	String(50))							 # 现工作负责人ID
	CHANGE_NEW_PRINCIPAL_UNAME	=	Column('CHANGE_NEW_PRINCIPAL_UNAME',String(50))							 # 现工作负责人
	ADD_CONTENT_PERMISSION_UID	=	Column('ADD_CONTENT_PERMISSION_UID',String(50))							 # 工作许可人ID
	ADD_CONTENT_PERMISSION_UNAME=	Column('ADD_CONTENT_PERMISSION_UNAME',String(50))						 # 工作许可人
	CHANGE_TIME					=	Column('CHANGE_TIME',				Date()	)							 # 同意变更时间
	DELAY_TIME					=	Column('DELAY_TIME',				Date()	)							 # 有效期延长到
	WRITE_TIME					=	Column('WRITE_TIME',				Date()	)							 # 填写时间
	ADD_CONTENT_DETAIL			=	Column('ADD_CONTENT_DETAIL',		String(50))							 # 增加的工作内容
	ADD_CONTENT_DETAIL_TIME		=	Column('ADD_CONTENT_DETAIL_TIME',	Date())								 # 增加的工作内容时间
	SAFE_GIVE_TIME				=	Column('SAFE_GIVE_TIME',			Date())								 # 安全交代时间
	WHETHER_QUALIFIED			=	Column('WHETHER_QUALIFIED',			String(50))			#Boolean()		 # 是否合格
	REMAKE						=	Column('REMAKE',					String(50))							 # 备注

class WorkTicket_Class_Three(Base):
	__tablename__ = 'BUSS_WORK_TICKET_3'
	
	
	WORK_WRITE_ID				=	Column('WORK_WRITE_ID',				String(50), primary_key=True)		 # 厂站第三种工作票ID
	WORK_WRITE_NO				=	Column('WORK_WRITE_NO',				String(50))							 # 工作票票号
	WHETHER_EXTERIOR			=	Column('WHETHER_EXTERIOR',			String(50))							 # 是否外来单位
	WORKBILL					=	Column('WORKBILL',					String(50))							 # 工单
	DH_WRITE_NO					=	Column('DH_WRITE_NO',				String(50))							 # 动火工作票票号
	DH_WRITE_NAME				=	Column('DH_WRITE_NAME',				String(50))							 # 动火工作票名称
	DEPARTMENT_NAME				=	Column('DEPARTMENT_NAME',			String(50))							 # 单位和班组
	SUBSTATION_TYPE				=	Column('SUBSTATION_TYPE',			String(50))							 # 厂站类型
	SUBSTATION_ID				=	Column('SUBSTATION_ID',				String(50))							 # 厂站ID
	SUBSTATION_NAME				=	Column('SUBSTATION_NAME',			String(50))							 # 厂站
	OPERATION_UNIT				=	Column('OPERATION_UNIT',			String(50))							 # 运维单位
	WORK_MASTER_UID				=	Column('WORK_MASTER_UID',			String(50))							 # 工作负责人ID
	WORK_MASTER_UNAME			=	Column('WORK_MASTER_UNAME',			String(50))							 # 工作负责人姓名
	WORK_MASTER_UPHONE			=	Column('WORK_MASTER_UPHONE',		String(50))							 # 负责人电话
	PLAN_BEGIN_DATE				=	Column('PLAN_BEGIN_DATE',			Date()	)							 # 计划开始时间
	PLAN_END_TIME				=	Column('PLAN_END_TIME',				Date()	)							 # 计划结束时间
	WORK_MEMBER					=	Column('WORK_MEMBER',				String(50))							 # 工作班人员
	WORK_TASK					=	Column('WORK_TASK',					String(50))							 # 工作任务
	WORK_PLACE					=	Column('WORK_PLACE',				String(50))							 # 工作地点
	SAFTY_AND_CARE				=	Column('SAFTY_AND_CARE',			String(50))							 # 工作要求的安全措施
	WATCH_UID					=	Column('WATCH_UID',					String(50))								 # 值班负责人ID
	WATCH_UNAME					=	Column('WATCH_UNAME',				String(50))							 # 值班负责人
	RECEIVE_TIME				=	Column('RECEIVE_TIME',				Date()	)							 # 收到工作票的时间
	WHETHER_MEET_SAFTY			=	Column('WHETHER_MEET_SAFTY',		String(50))			#Boolean()		 # 是否满足工作要求的安全措施
	SUPPLEMENT_SAFTY			=	Column('SUPPLEMENT_SAFTY',			String(50))							 # 需补充或调整的安全措施
	ELE_GENERATRIX_WIRE			=	Column('ELE_GENERATRIX_WIRE',		String(50))							 # 带电的母线、导线
	ELE_SWITCH					=	Column('ELE_SWITCH',				String(50))							 # 带电的隔离开关
	ELE_PART					=	Column('ELE_PART',					String(50))							 # 其他保留的带电部位
	PERMISSION_OTHER_CARE		=	Column('PERMISSION_OTHER_CARE',		String(50))							 # 其他安全注意事项
	PERMISSION_TIME				=	Column('PERMISSION_TIME',			Date())								 # 现场满足工作要求时间
	WORK_PERMISSION_UID			=	Column('WORK_PERMISSION_UID',		String(50))							 # 工作许可人ID
	WORK_PERMISSION_UNME		=	Column('WORK_PERMISSION_UNME',		String(50))							 # 工作许可人
	WORK_END_TIME				=	Column('WORK_END_TIME',				Date()	)							 # 工作结束时间
	CHANGE_SIGN_UID				=	Column('CHANGE_SIGN_UID',			String(50))							 # 工作票签发人ID
	CHANGE_SIGN_UNAME			=	Column('CHANGE_SIGN_UNAME',			String(50))							 # 工作票签发人
	CHANGE_PRINCIPAL_UID		=	Column('CHANGE_PRINCIPAL_UID',		String(50))							 # 原工作负责人ID
	CHANGE_PRINCIPAL_UNAME		=	Column('CHANGE_PRINCIPAL_UNAME',	String(50))							 # 原工作负责人
	CHANGE_NEW_PRINCIPAL_UID	=	Column('CHANGE_NEW_PRINCIPAL_UID',	String(50))							 # 现工作负责人ID
	CHANGE_NEW_PRINCIPAL_UNAME	=	Column('CHANGE_NEW_PRINCIPAL_UNAME',String(50))							 # 现工作负责人
	ADD_CONTENT_PERMISSION_UID	=	Column('ADD_CONTENT_PERMISSION_UID',String(50))							 # 工作许可人ID
	ADD_CONTENT_PERMISSION_UNAME=	Column('ADD_CONTENT_PERMISSION_UNAME',String(50))						 # 工作许可人
	CHANGE_TIME					=	Column('CHANGE_TIME',				Date()	)							 # 同意变更时间
	DELAY_TIME					=	Column('DELAY_TIME',				Date()	)							 # 有效期延长到
	WRITE_TIME					=	Column('WRITE_TIME',				Date())								 # 填写时间
	ADD_CONTENT_DETAIL			=	Column('ADD_CONTENT_DETAIL',		String(50))							 # 增加的工作内容
	ADD_CONTENT_DETAIL_TIME		=	Column('ADD_CONTENT_DETAIL_TIME',	Date())								 # 增加的工作内容时间
	SAFE_GIVE_TIME				=	Column('SAFE_GIVE_TIME',			Date()	)							 # 安全交代时间
	WHETHER_QUALIFIED			=	Column('WHETHER_QUALIFIED',			String(50))			#Boolean()		 # 是否合格
	REMAKE						=	Column('REMAKE',					String(50))							 # 备注


engine = create_engine(DATA_SOURCE_NAME, echo=True)
Database = sessionmaker(bind=engine)